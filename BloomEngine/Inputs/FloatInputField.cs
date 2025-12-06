using UnityEngine.UI;

namespace BloomEngine.Inputs;

public class FloatInputField : InputFieldBase<float>
{
    public Slider Slider => (Slider)Convert.ChangeType(InputObject, InputObjectType);

    public override void UpdateValue() => Value = Slider.value;
    public override void RefreshUI() => Slider.SetValueWithoutNotify(Value);
}