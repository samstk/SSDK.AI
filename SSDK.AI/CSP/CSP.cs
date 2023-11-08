using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.CSP
{
    /// <summary>
    /// Represents an instance of a constraint satisfaction problem.
    /// </summary>
    public abstract class CSP
    {
        #region Properties & Fields
        /// <summary>
        /// Gets or sets the variables that this CSP is interested in solving.
        /// </summary>
        public List<CSPVariable> Variables { get; set; } = new List<CSPVariable>();

        /// <summary>
        /// Gets or sets the constraints of this satisfaction problem.
        /// </summary>
        public List<CSPConstraint> Constraints { get; set; } = new List<CSPConstraint>();

        /// <summary>
        /// Checks the assignment of variables against every constraint.
        /// Does not consider unassigned values.
        /// </summary>
        public bool IsValid
        {
            get
            {
                foreach(CSPConstraint constraint in Constraints)
                {
                    if (!constraint.Holds)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// True if the CSP was solved.
        /// </summary>
        public bool Solved
        {
            get; private set;
        } = false;

        /// <summary>
        /// Checks whether all variables has a solution.
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return Variables.All((v) => v.Solved && v.Solution != null);
            }
        }
        #endregion

        /// <summary>
        /// Adds a unique constraint for a list of variables (i.e. each 
        /// variable must not intersect).
        /// </summary>
        /// <remarks>
        /// This constraint may require a variable's solution to be disjoint within the given list,
        /// however it does impose a constraint requiring a value to be present (see AddContainmentConstraint),
        /// which may speed-up the solving of the CSP. Only effective on solved variables.
        /// <br/>
        /// This constraint contains no hint of solving variables.
        /// </remarks>
        /// <param name="variables">the variables which must be distinct from each other</param>
        public void AddUniqueConstraint(CSPVariable[] variables)
        {
            Constraints.Add(new CSPConstraint(() =>
            {
                HashSet<object> values = new HashSet<object>();
                foreach(CSPVariable variable in variables)
                {
                    if (variable.Solution != null)
                    {
                        if (values.Contains(variable.Solution))
                            return false;
                        else values.Add(variable.Solution);
                    }
                }
                return true;
            }, variables, name: "Uniqueness Constraint"));
        }

        /// <summary>
        /// Adds a containment constraint which ensures every domain value
        /// is contained at least once within the variables.
        /// </summary>
        /// <remarks>
        /// Despite being added in a single function, the number of constraints added
        /// is equal to the length of the domain values list.
        /// <br/>
        /// This constraint contains a hint for solving variables by elimination
        /// (i.e. if no other variable can contain the value, then a particular
        /// variable must contain the value)
        /// </remarks>
        /// <param name="variables">the variables to check</param>
        /// <param name="domainValues">the domain value to ensure is included</param>
        public void AddContainmentConstraint(CSPVariable[] variables, params object[] domainValues)
        {
            foreach (object obj in domainValues)
            {
                Constraints.Add(new CSPConstraint(() =>
                {
                    HashSet<object> values = new HashSet<object>();
                    foreach (CSPVariable variable in variables)
                    {
                        if (variable.Domain.Contains(obj))
                        {
                            return true;
                        }
                    }
                    return false;
                }, variables, () =>
                {
                    HashSet<object> values = new HashSet<object>();
                    int count = 0;
                    CSPVariable lastContainedVariable = null;
                    foreach (CSPVariable variable in variables)
                    {
                        if (variable.Domain.Contains(obj))
                        {
                            count++;
                            if (variable.Solution == null)
                            {
                                lastContainedVariable = variable;
                            }
                        }
                    }
                    if (count == 1)
                    {
                        return (lastContainedVariable, obj);
                    }
                    return (null,null);
                }, name: $"({obj}) Exists Constraint"));
            }
        }
        
        /// <summary>
        /// Adds a unique constraint for a list of variables (i.e. each 
        /// variable must not intersect).
        /// </summary>
        /// <param name="variables">the variables which must be distinct from each other</param>
        public void AddConstraint(CSPConstraint constraint)
        {
            Constraints.Add(constraint);
        }
        /// <summary>
        /// Trims the domains of every variable. If any constraints fails for a particular value of an existing domain,
        /// then it is removed from the domain.
        /// </summary>
        /// <remarks>
        /// Trimming domains must be done before solving.
        /// </remarks>
        public void ReduceVariableDomains()
        {
            foreach(CSPVariable variable in Variables)
            {
                variable.ReduceDomain();
            }
        }

        /// <summary>
        /// Attempts to solve the constraint satisfaction problem
        /// (i.e. the values of all variables inside the problem), giving
        /// the first solution.
        /// </summary>
        /// <remarks>
        /// Each CSP can only be solved once, due to removal of domains
        /// whilst solving.
        /// </remarks>
        public virtual void Solve()
        {
            Solved = false;
            int solved = 1;
            // Snapshots used to restore from backtracking.
            Stack<(CSPVariable, object, (CSPDomain, object)[])> assignments 
                = new Stack<(CSPVariable, object, (CSPDomain, object)[])>();
            while (solved > 0 || assignments.Count != 0)
            {
                resolve:
                solved = 0;
                
                // Step one, solve all single-value domains, and
                // reduce related domains from these solutions.
                int step_solved = 1;
                while (step_solved > 0)
                {
                    step_solved = 0;

                    // Solve for all single-value domains
                    foreach (CSPVariable variable in Variables)
                    {
                        if (!variable.Solved && variable.Domain.Count == 1)
                        {
                            step_solved++;
                            variable.SetSolution(variable.Domain.FirstValue());
                        }
                    }

                    if (step_solved > 0)
                    {
                        // Reduce related domains in variables
                        foreach (CSPVariable variable in Variables)
                        {
                            if (variable.Domain.Count == 1)
                            {
                                if (variable.Solved)
                                {
                                    variable.ReduceRelatedDomains();
                                }
                            }
                        }
                    }

                    solved += step_solved;
                }
                

                if (solved > 0)
                    continue;

                // Step two, calculate hints on all constraints.
                step_solved = 1;
                while (step_solved > 0) {
                    step_solved = 0;
                    foreach (CSPConstraint constraint in Constraints)
                    {
                        (CSPVariable variable, object solution) = constraint.GetHint();
                        if(variable != null)
                        {
                            step_solved++;
                            variable.SetSolution(solution);
                        }
                    }

                    if (step_solved > 0)
                    {
                        // Reduce related domains in variables
                        foreach (CSPVariable variable in Variables)
                        {
                            if (variable.Solved && variable.Domain.Count == 1)
                            {
                                variable.ReduceRelatedDomains();
                            }
                        }
                    }
                    solved += step_solved;
                }

                // If any hints were computed, then repeat loop to avoid
                // performance drops (more expensive step)
                if (solved > 0)
                    continue;

                if (assignments.Count > 0)
                {
                    // We want to be assured that our current assignment works,
                    // and if it doesn't we must rollback to an earlier snapshots.
                }

                // Step three. Single-step backtracking

                // Select the variable with the least domain values
                enterBacktrackStep:
                CSPVariable backtrackVariable = null;
                int btvIndex = -1;
                int minDomain = int.MaxValue;

                int index = 0;
                foreach(CSPVariable variable in Variables)
                {
                    if (!variable.Solved && variable.Domain.Count < minDomain
                        && variable.Domain.Count > 1 && variable.Solution == null)
                    {
                        minDomain = variable.Domain.Count;
                        backtrackVariable = variable;
                        btvIndex = index;
                    }
                    index++;
                }

                if (backtrackVariable == null)
                {
                    // There are no further steps and the problem is solved
                    // or impossible. If the problem is solved (i.e. all
                    // constraints check out and all variables are filled out), then our backtracking solution is correct.

                    if (!IsComplete)
                    {
                        // Some values were not assigned, hence the problem is unsolved, so we have to rollback.
                        if (assignments.Count == 0)
                        {
                            // There exists no first variable with has a potential value so the problem is impossible.
                            goto finalise;
                        }
                        else
                        {
                            // Attempt to backtrack
                            goto backtrack;
                        }
                    }
                    else
                    {
                        if (IsValid)
                        {
                            // Our solution exists at this backtracking route.
                            foreach(CSPVariable variable in Variables)
                            {
                                variable.SetSolution(variable.Solution);
                            }
                            Solved = true;
                            return;
                        }
                        else
                        {
                            // Attempt to backtrack
                            goto backtrack;
                        }
                    }
                }

                // Select the first domain value of that variable
                backtrackVariable.BacktrackingIndex++;
                if (backtrackVariable.BacktrackingIndex >= backtrackVariable.Domain.Count)
                    backtrackVariable.BacktrackingIndex = 0;

                object backtrackValue = backtrackVariable.Domain.ValueAt(backtrackVariable.BacktrackingIndex);

                CSPDomain btvDomain = backtrackVariable.Domain.Clone();
                // Update solution (temporary) to domain value
                backtrackVariable.SetSolution(backtrackValue);

                if (!IsValid)
                {
                    // Just attempted to assign an invalid value that violated a constraint,
                    // so remove this value.
                    backtrackVariable.Solved = false;
                    backtrackVariable.Solution = null;
                    backtrackVariable.Domain = btvDomain;
                    backtrackVariable.Domain.Remove(backtrackValue);
                    solved++;
                    continue;
                }

                // Create a 'snapshot' of the domains currently present in the variables.
                (CSPDomain, object)[] snapshot = new (CSPDomain, object)[Variables.Count];
                for (int i = 0; i < Variables.Count; i++)
                {
                    snapshot[i] = (Variables[i].Domain.Clone(), Variables[i].Solution);
                }

                snapshot[btvIndex] = (btvDomain, null);

                assignments.Push((backtrackVariable, backtrackValue, snapshot));

                // Reduce all relevant domains of the variable
                backtrackVariable.ReduceRelatedDomains();
                continue;

                backtrack:
                if (assignments.Count == 0)
                {
                    // Unsolved
                    goto finalise;
                }
                
                // The current problem may be an invalid branch
                // of our original problem, so we must backtrack.
                (CSPVariable btVariable, object assignment, (CSPDomain, object)[] lastSnapshot) = assignments.Pop();
                btVariable.Solution = null;
                btVariable.Solved = false;

                // Restore domains prior to assignment.
                for (int i = 0; i < Variables.Count; i++)
                {
                    CSPVariable variable = Variables[i];
                    (variable.Domain, variable.Solution) = lastSnapshot[i];
                    variable.Solved = variable.Solution != null;
                }

                if (btVariable.Domain.Count == 1)
                {
                    goto resolve; // Finalise solvables
                }
                else
                {
                    // Ensure this value never is picked again
                    // as there is no solution for it (in this branch)
                    btVariable.Domain.Remove(assignment);
                }
                goto enterBacktrackStep;
                finalise:
                break;
            }
        }
    }
}
