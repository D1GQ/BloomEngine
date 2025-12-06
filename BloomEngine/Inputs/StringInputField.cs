using Il2CppReloaded.Input;

namespace BloomEngine.Inputs;

public class StringInputField : InputFieldBase<string>
{
    public ReloadedInputField Textbox => (ReloadedInputField)Convert.ChangeType(InputObject, InputObjectType);

    public override void UpdateValue() => Value = Textbox.text;
    public override void RefreshUI() => Textbox.SetTextWithoutNotify(Value);
}