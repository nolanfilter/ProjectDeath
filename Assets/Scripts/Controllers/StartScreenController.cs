using UnityEngine;
using System.Collections;

public class StartScreenController : MonoBehaviour {

	public GameObject startText;
	public GameObject creditsText;
	//public GameObject titleText;
	public GameObject creditsPrefab;

	private InputController inputController;

	private TextMesh start;
	private TextMesh credits;
	//private TextMesh title;

	private int select = 0;
	private bool creditsShown = false;
	private GameObject creditsSet;

	void Awake()
	{
		inputController = GetComponent<InputController>(); 
	}

	void Start () {
		start = startText.GetComponent<TextMesh>();
		credits = creditsText.GetComponent<TextMesh>();

		start.renderer.material.color = Color.white;
		credits.renderer.material.color = new Color(.1f,.4f,.41f,1f);
	}

	void OnEnable()
	{
		if( inputController )
		{
			inputController.OnButtonDown += OnButtonDown;
			inputController.OnButtonHeld += OnButtonHeld;
			inputController.OnButtonUp += OnButtonUp;	
		}
	}
	
	void OnDisable()
	{
		if( inputController )
		{
			inputController.OnButtonDown -= OnButtonDown;
			inputController.OnButtonHeld -= OnButtonHeld;
			inputController.OnButtonUp -= OnButtonUp;
		}
	}

	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{
		if( button == InputController.ButtonType.Up && select > 0)
		{
			if( !creditsShown )
			{
				select--;

				start.renderer.material.color = Color.white;
				credits.renderer.material.color = new Color(.1f,.4f,.41f,1f);
			}
		}
		else if( button == InputController.ButtonType.Down && select < 1 )
		{
			if( !creditsShown )
			{
				select++;

				start.renderer.material.color = new Color(.1f,.4f,.41f,1f);
				credits.renderer.material.color = Color.white;
			}
		}
		else if( button == InputController.ButtonType.Start )
		{
			if( creditsShown )
			{
				creditsShown = false;
				Destroy(creditsSet);
				start.text = "Start";
				credits.text = "Credits";
			}
			else
			{
				if( select == 0 )
				{
					Application.LoadLevel("MaybeTheGameMaybe");
				}
				else if( select == 1 )
				{
					creditsShown = true;
					creditsSet = Instantiate(creditsPrefab, new Vector3 (-3f, 0f, -7f), Quaternion.identity) as GameObject; //instantiate credits object
				}
			}
		}
	}
	
	private void OnButtonHeld( InputController.ButtonType button )
	{	

	}
	
	private void OnButtonUp( InputController.ButtonType button )
	{

	}
	//end event handlers/
}
