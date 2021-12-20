using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Puzzle : MonoBehaviour
{
	public MeshRenderer original;
	public float targetDistance = 0.5f;
	public string puzzleTag = "GameController";
	public int columns = 3;
	public int lines = 3;
	public float smooth = 5; 

	private int puzzleCounter, sortingOrder;
	private List<SpriteRenderer> _puzzle = new List<SpriteRenderer>();
	private List<Vector3> _puzzlePos = new List<Vector3>();
	private Transform current;
	private Vector3 offset;
	private bool isWinner;

	void Start()
	{
		Invoke("NewGame", 2);
	}

	void NewGame()
	{
		original.gameObject.SetActive(true);
		Clear();
		StartCoroutine(Generate());
	}

	int CheckPuzzle(float distance)
	{
		int i = 0;
		for (int j = 0; j < _puzzle.Count; j++)
		{
			if (Vector3.Distance(_puzzle[j].transform.position, _puzzlePos[j]) < distance)
			{
				i++;
			}
		}
		return i;
	}

	void Update()
	{
		if (isWinner)
		{
			if (CheckPuzzle(0.5f) == _puzzle.Count)
			{
				Clear();
				original.gameObject.SetActive(true);
			}
			else
			{
				for (int j = 0; j < _puzzle.Count; j++)
				{
					_puzzle[j].transform.position = Vector3.Lerp(_puzzle[j].transform.position, _puzzlePos[j], smooth * Time.deltaTime);
				}
			}
		}
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				GetPuzzle();
			}
			else if (Input.GetMouseButtonUp(0) && current)
			{
				current.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
				current = null;

				if (CheckPuzzle(targetDistance) == _puzzle.Count)
				{
					isWinner = true;
					SceneManager.UnloadSceneAsync("MiniGameLock_1");
				}
			}
		}

		if (current)
		{
			Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			current.position = new Vector3(position.x, position.y, current.position.z) + new Vector3(offset.x, offset.y, 0);
		}
	}

	
	 void Clear()
	{
		isWinner = false;
		puzzleCounter = 0;
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		_puzzle = new List<SpriteRenderer>();
		_puzzlePos = new List<Vector3>();
	}
	 

	void Randomize()
	{
		float[] x = new float[_puzzle.Count];
		float[] y = new float[_puzzle.Count];

		for (int j = 0; j < _puzzle.Count; j++)
		{
			x[j] = _puzzlePos[j].x;
			y[j] = _puzzlePos[j].y;
		}

		float minX = Mathf.Min(x);
		float maxX = Mathf.Max(x);
		float minY = Mathf.Min(y);
		float maxY = Mathf.Max(y);

		foreach (SpriteRenderer e in _puzzle)
		{
			e.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), e.transform.position.z);
		}
	}

	IEnumerator Generate() 
	{
		Vector3 posStart = Camera.main.WorldToScreenPoint(new Vector3(original.bounds.min.x, original.bounds.min.y, original.bounds.min.z));
		Vector3 posEnd = Camera.main.WorldToScreenPoint(new Vector3(original.bounds.max.x, original.bounds.max.y, original.bounds.min.z));

		int width = (int)(posEnd.x - posStart.x);
		int height = (int)(posEnd.y - posStart.y);

		int w_cell = width / columns;
		int h_cell = height / lines;

		int xAdd = (Screen.width - width) / 2;
		int yAdd = (Screen.height - height) / 2;

		yield return new WaitForEndOfFrame();

		for (int y = 0; y < lines; y++)
		{
			for (int x = 0; x < columns; x++)
			{
				Rect rect = new Rect(0, 0, w_cell, h_cell);
				rect.center = new Vector2((w_cell * x + w_cell / 2) + xAdd, (h_cell * y + h_cell / 2) + yAdd);
				Vector3 position = Camera.main.ScreenToWorldPoint(rect.center);
				Texture2D tex = new Texture2D(w_cell, h_cell, TextureFormat.ARGB32, false);
				tex.ReadPixels(rect, 0, 0);
				tex.Apply();

				GameObject obj = new GameObject("Puzzle: " + puzzleCounter);
				obj.transform.parent = transform;
				position = new Vector3(((int)(position.x * 100f)) / 100f, ((int)(position.y * 100f)) / 100f, puzzleCounter / 100f);
				obj.transform.localPosition = position;
				
				SpriteRenderer ren = obj.AddComponent<SpriteRenderer>();
				int unit = Mathf.RoundToInt((float)Screen.height / (Camera.main.orthographicSize * 2f)); // формула расчета размера спрайта (только для режима камеры Оrthographic)
				ren.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), unit);
				obj.AddComponent<BoxCollider2D>();
				obj.tag = puzzleTag;

				_puzzlePos.Add(position);
				_puzzle.Add(ren);
				puzzleCounter++;
			}
		}

		original.gameObject.SetActive(false);
		Randomize();
	}

	void GetPuzzle()
	{		
		RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.Length > 0 && hit[0].transform.tag == puzzleTag)
		{
			offset = hit[0].transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
			current = hit[0].transform;
			sortingOrder = current.GetComponent<SpriteRenderer>().sortingOrder;
			current.GetComponent<SpriteRenderer>().sortingOrder = puzzleCounter + 1;
		}
	}
}