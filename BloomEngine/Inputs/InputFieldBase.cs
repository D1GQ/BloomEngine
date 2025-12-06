namespace BloomEngine.Inputs;

public abstract class InputFieldBase<T> : IInputField<T>
{
    public Type ValueType => Value.GetType();
    public T Value
    {
        get => field;
        set
        {
            field = TransformValue is not null ? TransformValue.Invoke(value) : value;
            OnValueChanged?.Invoke(field);
        }
    }

    public object InputObject { get; set; }
    public Type InputObjectType => InputObject.GetType();

    public Action<T> OnValueChanged { get; set; }
    public Func<T, T> TransformValue { get; set; }
    public Func<T, bool> ValidateValue { get; set; }

    public object GetValueObject() => Value;
    public void SetValueObject(object value) => Value = (T)Convert.ChangeType(value, typeof(T));

    public abstract void UpdateValue();
    public abstract void RefreshUI();
    public virtual void OnUIChanged() { }
}