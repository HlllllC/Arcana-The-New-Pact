using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView HandView;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CardView cardView = CardViewCreator.Instance.CreateCardView(transform.position, Quaternion.identity);
            cardView.transform.SetParent(HandView.transform);
            StartCoroutine(HandView.AddCard(cardView));
        }
    }
}
