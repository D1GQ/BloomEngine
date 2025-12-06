using Il2CppSource.UI;

namespace BloomEngine.Inputs;

public class EnumInputField : InputFieldBase<Enum>
{
    public ReloadedDropdown Dropdown => (ReloadedDropdown)Convert.ChangeType(InputObject, InputObjectType);

    public override void UpdateValue() => Value = GetOptions()[Dropdown.value];
    public override void RefreshUI()
    {
        Dropdown.SetValueWithoutNotify(GetOptions().IndexOf(Value));
        Dropdown.RefreshShownValue();
    }


    private List<Enum> GetOptions() => Enum.GetValues(Value.GetType()).Cast<Enum>().ToList();
}