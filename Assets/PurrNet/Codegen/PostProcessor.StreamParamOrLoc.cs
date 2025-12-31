#if UNITY_MONO_CECIL
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace PurrNet.Codegen
{
    public readonly struct StreamParamOrLoc
    {
        public readonly ParameterDefinition streamParam;
        public readonly VariableDefinition streamLoc;

        public StreamParamOrLoc(ParameterDefinition streamParam)
        {
            this.streamParam = streamParam;
            this.streamLoc = null;
        }

        public StreamParamOrLoc(VariableDefinition streamLoc)
        {
            this.streamLoc = streamLoc;
            this.streamParam = null;
        }

        public Instruction Load()
        {
            return streamParam != null
                ? Instruction.Create(OpCodes.Ldarg, streamParam)
                : Instruction.Create(OpCodes.Ldloc, streamLoc);
        }
    }
}
#endif
