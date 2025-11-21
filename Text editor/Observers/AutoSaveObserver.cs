using System;
using System.Diagnostics; 

namespace TextEditorMK.Observers
{
    public class AutoSaveObserver : IObserver
    {

        public void Update(string textContent)
        {
            try
            {
                Debug.WriteLine($"[AutoSaveObserver] {DateTime.Now}: Зміни зафіксовано. Довжина тексту: {textContent.Length}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving: {ex.Message}");
            }
        }
    }
}
