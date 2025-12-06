using BloomEngine.Utilities;
using Il2CppReloaded.Input;

namespace BloomEngine.Inputs;

public class IntInputField : InputFieldBase<int>
{
    public ReloadedInputField Textbox => (ReloadedInputField) Convert.ChangeType(InputObject, InputObjectType);

    public override void UpdateValue() => Value = (int)TextHelper.ValidateNumericInput(Textbox.text, typeof(int));
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value.ToString());
    public override void OnUIChanged() => Textbox.SetTextWithoutNotify(TextHelper.SanitiseNumericInput(Textbox.text));
}