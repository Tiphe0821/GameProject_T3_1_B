using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Gameplay;

public class CraftingStation : MonoBehaviour
{
    [Serializable]
    public class Ingredient
    {
        public string itemId;
        public int amount = 1;

        public Ingredient() { }                                  // 인스펙터용 기본 생성자

        public Ingredient(string itemId, int amount)             // 코드에서 편하게 만들기 위한 생성자
        {
            this.itemId = itemId;
            this.amount = amount;
        }
    }

    [Header("제작 레시피 (재료)")]
    [SerializeField] private List<Ingredient> ingredients = new List<Ingredient>();

    [Header("결과물")]
    [SerializeField] private string resultItemId = "";
    [SerializeField] private int resultAmount = 1;

    // ===================== 레시피 읽기 (UI 등에서 현재 레시피를 확인할 때) =====================

    public IReadOnlyList<Ingredient> Ingredients => ingredients;
    public string ResultItemId => resultItemId;
    public int ResultAmount => resultAmount;

    // ===================== 레시피 변경 =====================

    // 레시피 전체를 한 번에 바꾼다 (기존 재료는 모두 지워진다)
    // 사용 예)
    //   station.SetRecipe("tool", 1,
    //       new CraftingStation.Ingredient("iron", 2),
    //       new CraftingStation.Ingredient("wood", 1));
    public void SetRecipe(string newResultItemId, int newResultAmount, params Ingredient[] newIngredients)
    {
        SetResult(newResultItemId, newResultAmount);

        ingredients.Clear();

        if (newIngredients == null) return;

        foreach (Ingredient ingredient in newIngredients)
        {
            if (ingredient == null) continue;
            AddIngredient(ingredient.itemId, ingredient.amount);
        }
    }

    // 결과물만 바꾼다
    public void SetResult(string newResultItemId, int newResultAmount)
    {
        resultItemId = newResultItemId;
        resultAmount = newResultAmount;
    }

    // 재료를 하나 추가한다 (이미 있는 재료면 개수만 늘린다)
    public void AddIngredient(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return;

        foreach (Ingredient existing in ingredients)
        {
            if (existing.itemId == itemId)
            {
                existing.amount += amount;
                return;
            }
        }

        ingredients.Add(new Ingredient(itemId, amount));
    }

    // 특정 재료의 개수를 바꾼다. 개수가 0 이하면 그 재료를 레시피에서 뺀다
    public void SetIngredientAmount(string itemId, int amount)
    {
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (ingredients[i].itemId != itemId) continue;

            if (amount <= 0) ingredients.RemoveAt(i);
            else ingredients[i].amount = amount;
            return;
        }

        // 레시피에 없던 재료라면 새로 추가한다
        if (amount > 0) ingredients.Add(new Ingredient(itemId, amount));
    }

    // 특정 재료를 레시피에서 뺀다
    public void RemoveIngredient(string itemId)
    {
        ingredients.RemoveAll(ingredient => ingredient.itemId == itemId);
    }

    // 재료를 전부 지운다
    public void ClearIngredients()
    {
        ingredients.Clear();
    }

    // ===================== 제작 =====================

    // 제작에 성공하면 true
    public bool TryCraft()
    {
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null) return false;

        if (string.IsNullOrEmpty(resultItemId) || resultAmount <= 0)
        {
            Debug.LogWarning("[제작] 결과물 아이템 id 또는 수량이 설정되지 않았습니다.");
            return false;
        }

        // 결과물이 ItemDatabase에 없으면 재료만 사라지므로, 먼저 확인하고 취소한다
        if (inventory.Database == null || inventory.Database.Get(resultItemId) == null)
        {
            Debug.LogWarning($"[제작] 결과물 '{resultItemId}'이(가) ItemDatabase에 등록되어 있지 않습니다.");
            return false;
        }

        // 같은 재료가 여러 줄에 적혀 있어도 합산해서 계산한다
        Dictionary<string, int> required = new Dictionary<string, int>();
        foreach (Ingredient ingredient in ingredients)
        {
            if (string.IsNullOrEmpty(ingredient.itemId) || ingredient.amount <= 0) continue;

            required.TryGetValue(ingredient.itemId, out int current);
            required[ingredient.itemId] = current + ingredient.amount;
        }

        if (required.Count == 0)
        {
            Debug.LogWarning("[제작] 재료가 설정되지 않았습니다.");
            return false;
        }

        // 2. 수량이 부족하면 제작을 취소한다 (아이템은 하나도 제거하지 않는다)
        foreach (KeyValuePair<string, int> pair in required)
        {
            if (!inventory.Has(pair.Key, pair.Value))
            {
                Debug.Log($"[제작] 재료 부족: {pair.Key} (필요 {pair.Value}개 / 보유 {inventory.GetCount(pair.Key)}개)");
                return false;
            }
        }

        // 1. 재료를 인벤토리에서 제거한다
        foreach (KeyValuePair<string, int> pair in required)
        {
            inventory.RemoveItem(pair.Key, pair.Value);
        }

        // 3. 결과물을 인벤토리에 추가한다
        if (!inventory.AddItem(resultItemId, resultAmount))
        {
            // 만약 추가에 실패하면 재료를 돌려준다
            foreach (KeyValuePair<string, int> pair in required)
            {
                inventory.AddItem(pair.Key, pair.Value);
            }
            return false;
        }

        Debug.Log($"[제작] 성공: {resultItemId} x{resultAmount}");
        return true;
    }
}