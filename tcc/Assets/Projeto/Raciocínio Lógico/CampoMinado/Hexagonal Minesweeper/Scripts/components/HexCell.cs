using UnityEngine;

public class HexCell : MonoBehaviour {
	//different user defined colors
	public Color baseMarkedColor;
	public Color baseMineColor;
	public Color baseHiddenColor;
	public Color baseRevealedColor;
	public Color borderColor;
	public Vector2 axialCoordinate=new Vector2();//this cell's axial coordinate values
	public bool isMarked;//whether the cell is currently marked as mine
	public bool isMine;//whether this is mine of not
	public bool isRevealed;//whether this tile is hidden or revealed

	//sprite renderers of sprites
	public SpriteRenderer hexBorder_SR;
	public SpriteRenderer inner_SR;
	public SpriteRenderer hexBase_SR;
	public Sprite innerSprite;//inner image to be set via code
	public Sprite flagSprite;//the flag image

	// Use this for initialization
	void Awake () {
		reset();
	}
	public void setVertical(){//we need to rotate the border & base by 90 degrees if vertically aligned
		GameObject border_GO=hexBorder_SR.gameObject;
		GameObject base_GO=hexBase_SR.gameObject;
		border_GO.transform.rotation = Quaternion.Euler (0, 0, 90);
		base_GO.transform.rotation = Quaternion.Euler (0, 0, 90);
	}
	public void reset(){//reset all colors & set to not marked
		hexBorder_SR.color=borderColor;
		setBaseColor(baseHiddenColor);
		isMarked = false;//set marked status
		inner_SR.sprite = null;//remove any sprite shown inside (mine, flag)
		isRevealed = false;//set revealed status
	}
	public void setMine(){
		isMine = true;
	}
	public void reveal(){
		if (isMine) {//set base color
			setBaseColor(baseMineColor);
		} else {//
			setBaseColor(baseRevealedColor);
		}
		inner_SR.sprite = innerSprite;
		isRevealed = true;//set reveal status
	}
	public void markBase(){//flag the cell as marked
		setBaseColor(baseMarkedColor);
		inner_SR.sprite = flagSprite;//show flag
		isMarked = true;//set marked status
	}
	void setBaseColor(Color c){
		hexBase_SR.color=c;
	}
}
