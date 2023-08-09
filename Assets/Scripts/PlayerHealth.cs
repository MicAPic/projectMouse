using UnityEditor;

public class PlayerHealth : Health
{
    protected override void Die() 
    {
        EditorApplication.ExitPlaymode();
    }
}