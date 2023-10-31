using SSDK.AI.Agent.Info;
using SSDK.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.Agent.Info
{
    /// <summary>
    /// Contains information relevant to the environment that an agent acts within.
    /// </summary>
    public sealed class AgentInfo
    {
        /// <summary>
        /// Gets or sets the modularity information of the agent.
        /// </summary>
        public AgentModularity Modularity { get; set; } = AgentModularity.Undefined;
        /// <summary>
        /// Gets or sets the planning horizon information of the agent.
        /// </summary>
        public AgentPlanningHorizon PlanningHorizon { get; set; } = AgentPlanningHorizon.Undefined;
        /// <summary>
        /// Gets or sets the representation information of the agent.
        /// </summary>
        public AgentRepresentation Representation { get; set; } = AgentRepresentation.Undefined;
        /// <summary>
        /// Gets or sets the computational limit information of the agent.
        /// </summary>
        public AgentComputationalLimits ComputationalLimits { get; set; } = AgentComputationalLimits.Undefined;
        /// <summary>
        /// Gets or sets the learning type information of the agent.
        /// </summary>
        public AgentLearningType Learning { get; set; } = AgentLearningType.Undefined;
        /// <summary>
        /// Gets or sets the sensing uncertainty information of the agent.
        /// </summary>
        public AgentSensingUncertainty SensingUncertainty { get; set; } = AgentSensingUncertainty.Undefined;
        /// <summary>
        /// Gets or sets the effect uncertainty information of the agent.
        /// </summary>
        public AgentEffectUncertainty EffectUncertainty { get; set; } = AgentEffectUncertainty.Undefined;
        /// <summary>
        /// Gets or sets the preference information of the agent.
        /// </summary>
        public AgentPreferences Preference { get; set; } = AgentPreferences.Undefined;
        /// <summary>
        /// Gets or sets the coordination information of the agent.
        /// </summary>
        public AgentCoordination NumberOfAgents { get; set; } = AgentCoordination.Undefined;
        /// <summary>
        /// Gets or sets the interaction time information of the agent.
        /// </summary>
        public AgentInteractionTime Interaction { get; set; } = AgentInteractionTime.Undefined;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("           Dimension | Value");

            b.Append("          Modularity : ");
            b.Append(Modularity.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("    Planning Horizon : ");
            b.Append(PlanningHorizon.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("      Representation : ");
            b.Append(Representation.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("Computational Limits : ");
            b.Append(ComputationalLimits.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("            Learning : ");
            b.Append(Learning.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append(" Sensing Uncertainty : ");
            b.Append(SensingUncertainty.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("  Effect Uncertainty : ");
            b.Append(EffectUncertainty.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("          Preference : ");
            b.Append(Preference.ToString().SpaceOutCapitals());
            b.AppendLine();

            b.Append("         Interaction : ");
            b.Append(Interaction.ToString().SpaceOutCapitals());
            b.AppendLine();

            return b.ToString();
        }
    }
}
