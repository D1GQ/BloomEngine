namespace BloomEngine.Inputs;

public interface IInputField
{
    Type ValueType { get; }

    object InputObject { get; set; }
    Type InputObjectType { get; }

    object GetValueObject();
    void SetValueObject(object value);

    void UpdateValue();
    void RefreshUI();
    void OnUIChanged();
}

public interface IInputField<T> : IInputField
{
    Action<T> OnValueChanged { get; set; }
    Func<T, T> TransformValue { get; set; }
    Func<T, bool> ValidateValue { get; set; }

    T Value { get; set; }

    object GetValueObject();
    void SetValueObject(object value);

    void UpdateValue();
    void RefreshUI();
    void OnUIChanged();
}