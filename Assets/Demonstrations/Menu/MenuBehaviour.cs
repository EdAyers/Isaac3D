using UnityEngine;
using UnityEngine.UI;
using System.Collections;

///Script that handles the behaviour of a menu item.
/// - animation when looking at it
/// - load demo when tapped
public class MenuBehaviour : MonoBehaviour
{
  public int index = 0;
  public int levelNo = 0;
  public float selectDistance = 1.2f;
  Canvas canvas;
  Cardboard cardboard;
  Collider collider;
  Camera camera;
  Vector3 startPosition;
  bool isSelected = false;
  Color isaacColor;
  public Color selectColor;
  bool timing;
  float startTime;
  const float TIME_THR = 0.1f;
  
  void Start()
  {
    ////rotate the canvas to be pointing up
    //canvas = this.GetComponentInChildren<Canvas>();
    //canvas.transform.LookAt(transform.position * 3, new Vector3(0, 1, 0));

    cardboard = GameObject.Find("CardboardMain").GetComponent<Cardboard>();
    camera = cardboard.GetComponentInChildren<Camera>();
    collider = this.GetComponentInChildren<MeshCollider>();
    startPosition = transform.position;
    Color.TryParseHexString("44AA44FF", out isaacColor);
    selectColor = Color.Lerp(isaacColor, Color.white, 0.3f);
  }
  
  void Update()
  {
    RaycastHit hit;
    if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 500))
    {
      if (hit.collider == collider)
      {
        if (cardboard.CardboardTriggered)
        {
          if (levelNo == -1)
          {
            //send email
            string email = "cardboard@isaacphysics.org";
            string subject = WWW.EscapeURL("Feedback for Isaac 3D").Replace("+", "%20");
 
            Application.OpenURL("mailto:" + email + "?subject=" + subject);
 
          }
          else if (levelNo == -2)
          {
            //go to website
            //Debug.Log("opening website");
            Application.OpenURL("http://www.isaacphysics.org");
          }
          else
          {
            GetComponentInChildren<Text>().text = "LOADING...";
            Application.LoadLevel(levelNo);
          }
        }
        else
        {
          SelectConditionsSatisfied();
        }
        return;
      }

    }
    DeselectConditionsSatisfied();
  }
  
  ///Method that checks if predicate has been true
  ///for over `TIME_THR` seconds.
  bool DelayAction(bool predicate)
  {
    if (predicate)
    {
      if (timing && (Time.time - startTime) > TIME_THR)
      {
        timing = false;
        return true;
      }
      else if (!timing)
      {
        timing = true;
        startTime = Time.time;
        return false;
      }
      else
      {
        return false;
      }
    }
    else
    {
      timing = false;
      return false;
    }
  }
  
  void SelectConditionsSatisfied()
  {
    if (DelayAction(!isSelected))
    {
      isSelected = true;
      iTween.MoveBy(this.gameObject, Vector3.back * selectDistance, 0.2f);
      iTween.ColorTo(this.gameObject, selectColor, 0.2f);
    }
  }
  
  void DeselectConditionsSatisfied()
  {
    if (DelayAction(isSelected))
    {
      isSelected = false;
      iTween.MoveTo(this.gameObject, startPosition, 0.2f);
      iTween.ColorTo(this.gameObject, isaacColor, 0.2f);
    }
  }
}
