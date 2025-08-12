using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainContentPage : MonoBehaviour
{
	[SerializeField] private MainPagesType _page;
	public MainPagesType Page { get => _page; set => _page = value; }
}
