

// public class CombineTilesUndoAction : IUndoAction
// {
//     private Tile tileA;
//     private Tile tileB;
//     private ColorVector originalColorA;
//     private ColorVector originalColorB;
//     private bool hadResultColor; // Son tile'a ara renk eklenmiş miydi?

//     public CombineTilesUndoAction(Tile tileA, Tile tileB, ColorVector originalColorA, ColorVector originalColorB, bool hadResultColor)
//     {
//         this.tileA = tileA;
//         this.tileB = tileB;
//         this.originalColorA = originalColorA;
//         this.originalColorB = originalColorB;
//         this.hadResultColor = hadResultColor;
//     }

//     public void Undo()
//     {
//         // Eğer ara renk eklenmişse, onu sil
//         if (hadResultColor)
//         {
//             tileB.PopTopColor();
//         }

//         // Eski renkleri geri koy
//         tileA.PushColor(originalColorA);
//         tileB.PushColor(originalColorB);
//     }
// }
// using System.Collections.Generic;
// using UnityEngine;

// public interface IUndoAction
// {
//     void Undo();
// }

// public class UndoManager : MonoBehaviour
// {
//     public static UndoManager Instance { get; private set; }

//     private Stack<IUndoAction> undoStack = new Stack<IUndoAction>();
//     private const int maxUndoCount = 3;

//     private void Awake()
//     {
//         if (Instance == null)
//             Instance = this;
//         else
//             Destroy(gameObject);
//     }

//     public void RecordAction(IUndoAction action)
//     {
//         if (undoStack.Count >= maxUndoCount)
//         {
//             // En eski undo kaydını atmak için stack'ı Queue gibi kullanamayız, 
//             // o yüzden burada basitçe en alttakini atmak için yeni bir yapı gerekebilir.
//             // Ama stack ile max 3 için en kolay yol yeni stack oluşturup eski 2 sini taşımak:
//             var tempList = new List<IUndoAction>(undoStack);
//             tempList.RemoveAt(tempList.Count - 1); // En eskiyi çıkar (stack en son eklenen üstte, biz sondaki kaldırıyoruz)
//             undoStack.Clear();
//             for (int i = tempList.Count - 1; i >= 0; i--) // Yine stack mantığıyla ters ekle
//                 undoStack.Push(tempList[i]);
//         }

//         undoStack.Push(action);
//         Debug.Log($"Undo işlemi kaydedildi. Şu an {undoStack.Count} undo hakkı var.");
//     }

//     public void Undo()
//     {
//         if (undoStack.Count > 0)
//         {
//             var action = undoStack.Pop();
//             action.Undo();
//             Debug.Log($"Undo yapıldı. Kalan undo hakkı: {undoStack.Count}");
//         }
//         else
//         {
//             Debug.Log("Undo yapılacak işlem yok.");
//         }
//     }
// }