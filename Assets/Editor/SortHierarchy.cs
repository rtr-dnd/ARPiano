
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SortHierarchy : MonoBehaviour
{

    const string itemname_byname = "GameObject/Sort/By Name";
    const string itemname_byposx = "GameObject/Sort/By Position X";
    const string itemname_byposy = "GameObject/Sort/By Position Y";
    const string itemname_byposz = "GameObject/Sort/By Position Z";

    // アルファベット順に並べ替え
    [MenuItem(itemname_byname, false, 130)]
    static void SortByName()
    {
        Sort((a, b) => string.Compare(a.name, b.name));
    }

    // X座標の位置で並べ替え
    [MenuItem(itemname_byposx, false, 130)]
    static void SortByPosX()
    {
        Sort((a, b) => a.position.x.CompareTo(b.position.x));
    }

    // Y座標の位置で並べ替え
    [MenuItem(itemname_byposy, false, 130)]
    static void SortByPosY()
    {
        Sort((a, b) => a.position.y.CompareTo(b.position.y));
    }

    // Z座標の位置で並べ替え
    [MenuItem(itemname_byposz, false, 130)]
    static void SortByPosZ()
    {
        Sort((a, b) => a.position.z.CompareTo(b.position.z));
    }

    // 現在選択中のオブジェクトを指定した比較方法で並べ替える
    static void Sort(Comparison<Transform> compare)
    {
        var selected = Selection.transforms.GroupBy(s => s.parent);
        foreach (var group in selected)
        {
            var sorted = group.ToList();
            sorted.Sort(compare);

            var indices = sorted.Select(s => s.GetSiblingIndex()).OrderBy(s => s).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                Undo.SetTransformParent(sorted[i], sorted[i].parent, "Sort");
                sorted[i].SetSiblingIndex(indices[i]);
            }
        }
    }

    // オブジェクトが選択されていなければメニューを無効化する
    [MenuItem(itemname_byname, true)]
    [MenuItem(itemname_byposx, true)]
    [MenuItem(itemname_byposy, true)]
    [MenuItem(itemname_byposz, true)]
    static bool ValidateSort()
    {
        return Selection.transforms.Any();
    }
}