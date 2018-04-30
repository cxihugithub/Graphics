namespace UnityEditor.VFX.Operator
{
    [VFXInfo(category = "Math/Arithmetic", experimental = true)]
    class PowerNew : VFXOperatorNumericCascadedUnifiedNew
    {
        public override sealed string name { get { return "PowerNew"; } }
        protected override sealed double defaultValueDouble { get { return 1.0; } }
        protected override sealed ValidTypeRule typeFilter { get { return ValidTypeRule.allowEverythingExceptInteger; } }

        protected override sealed VFXExpression ComposeExpression(VFXExpression a, VFXExpression b)
        {
            return new VFXExpressionPow(a, b);
        }
    }
}
