using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
	public ChampionController owner;

	public List<Card> cards = new List<Card>();

	/// <summary>
	/// Sets `championController` as the owner of this hand.
	/// </summary>
	/// <param name="championController"></param>
	public void SetOwner(ChampionController championController) {
		owner = championController;
		championController.hand = this;

		name = owner.championName + "'s Hand";
	}
	
	// GetCard Functions
	/// <summary>
	/// Returns the amount of cards that this hand owns, identical to `hand.GetCardCount()` but supposedly filtered.
	/// </summary>
	/// <returns></returns>
	public int GetCardCount() {
		var cardCount = 0;

		foreach (Transform child in transform) {
			if (child.GetComponent<Card>() == null) continue;
			if (!cards.Contains(child.GetComponent<Card>())) continue;
			cardCount++;
		}

		return cardCount;
	}
	/// <summary>
	/// Get a specific card with a given criteria to search for.
	///
	/// Valid types are: "Lowest", "Highest", "Defense"
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public Card GetCard(string type) {
		Card selectedCard = null;
		int value;
		switch (type) {
			case "Lowest":
				value = 999;
				foreach (Transform child in transform) {
					if (child.GetComponent<Card>().cardScriptableObject.cardValue >= value) continue;
					selectedCard = child.GetComponent<Card>();
					value = selectedCard.cardScriptableObject.cardValue;
				}
				break;
			case "Highest":
				value = -999;
				foreach (Transform child in transform) {
					if (child.GetComponent<Card>().cardScriptableObject.cardValue <= value) continue;
					selectedCard = child.GetComponent<Card>();
					value = selectedCard.cardScriptableObject.cardValue;
				}
				break;
			case "Defense":
				value = -999;
				foreach (Transform child in transform) {
					switch (GameController.instance.difficulty) {
						case GameController.Difficulty.Noob:
							if (child.GetSiblingIndex() == child.parent.childCount - 1) selectedCard = child.GetComponent<Card>();
							if (Random.Range(0f, 1f) < 0.5f) continue;
							break;
						case GameController.Difficulty.Novice:
							if (child.GetSiblingIndex() == child.parent.childCount - 1) selectedCard = child.GetComponent<Card>();
							if (child.GetComponent<Card>().cardScriptableObject.cardValue <= 9) continue;
							break;
						case GameController.Difficulty.Warrior:
							if (owner.currentHP >= 0.3f * owner.maxHP && child.GetComponent<Card>().cardScriptableObject.cardValue >= 12 && Random.Range(0f, 1f) < 0.75f) {
								Debug.Log("The " + owner.championName + " is confident! They refuse to use a value of " + child.GetComponent<Card>().cardScriptableObject.cardValue + " to defend!");
								continue;
							}
							if (value < child.GetComponent<Card>().cardScriptableObject.cardValue) {
								selectedCard = child.GetComponent<Card>();
								value = selectedCard.cardScriptableObject.cardValue;
							}
							break;
						case GameController.Difficulty.Champion:
							if (owner.currentHP >= 0.5f * owner.maxHP && child.GetComponent<Card>().cardScriptableObject.cardValue >= 12 && Random.Range(0f, 1f) < 0.75f) {
								Debug.Log("The " + owner.championName + " is confident! They refuse to use a value of " + child.GetComponent<Card>().cardScriptableObject.cardValue + " to defend!");
								continue;
							}
							if (value < child.GetComponent<Card>().cardScriptableObject.cardValue) {
								selectedCard = child.GetComponent<Card>();
								value = selectedCard.cardScriptableObject.cardValue;
							}
							break;
					}
				}
				break;
			default:
				Debug.LogError("No GetCard type was specified!");
				return null;
		}

		if (selectedCard == null) {
			Debug.LogWarning("No card within criteria was found! Returning a null.");
			return null;
		}
		return selectedCard;
	}
	public Card GetCard(string type, CardSuit suitCriteria) {
		Card selectedCard = null;
		int value;
		switch (type) {
			case "Lowest":
				value = 999;
				foreach (Transform child in transform) {
					if (child.GetComponent<Card>().cardScriptableObject.cardValue >= value || child.GetComponent<Card>().cardScriptableObject.cardSuit != suitCriteria) continue;
					selectedCard = child.GetComponent<Card>();
					value = selectedCard.cardScriptableObject.cardValue;
				}
				break;
			case "Highest":
				value = -999;
				foreach (Transform child in transform) {
					if (child.GetComponent<Card>().cardScriptableObject.cardValue <= value || child.GetComponent<Card>().cardScriptableObject.cardSuit != suitCriteria) continue;
					selectedCard = child.GetComponent<Card>();
					value = selectedCard.cardScriptableObject.cardValue;
				}
				break;
			default:
				Debug.LogError("No GetCard type was specified!");
				return null;
		}

		if (selectedCard == null) {
			Debug.LogWarning("No card within criteria was found! Returning a null.");
			return null;
		}
		return selectedCard;
	}
	/// <summary>
	/// A smarter coherence designed to return a card specifically for attacking.
	/// </summary>
	/// <param name="card"></param>
	/// <returns></returns>
	public Card GetAttackingCard(Card card) {
		Card selectedCard = null;
		var value = -999;
		foreach (Transform child in transform) {
			if (child.GetComponent<Card>() == card) continue;

			switch (GameController.instance.difficulty) {
				case GameController.Difficulty.Noob:
					if (child.GetSiblingIndex() == child.parent.childCount - 1) selectedCard = child.GetComponent<Card>();
					if (Random.Range(0f, 1f) < 0.5f) continue;
					break;
				case GameController.Difficulty.Novice:
					if (child.GetSiblingIndex() == child.parent.childCount - 1) selectedCard = child.GetComponent<Card>();
					if (child.GetComponent<Card>().cardScriptableObject.cardValue <= 9) continue;
					break;
				case GameController.Difficulty.Warrior:
					if (owner.currentHP >= 0.25f * owner.maxHP && child.GetComponent<Card>().cardScriptableObject.cardValue >= 10 && Random.Range(0f, 1f) < 0.75f) {
						Debug.Log("The opponent is confident! They refuse to use a value of " + child.GetComponent<Card>().cardScriptableObject.cardValue + " to attack!");
						continue;
					}
					if (value < child.GetComponent<Card>().cardScriptableObject.cardValue) {
						selectedCard = child.GetComponent<Card>();
						value = selectedCard.cardScriptableObject.cardValue;
					}
					break;
				case GameController.Difficulty.Champion:
					if (child.GetComponent<Card>().cardScriptableObject.cardSuit == CardSuit.HEART && owner.currentHP <= 0.75f * owner.maxHP && Random.Range(0f, 1f) < 0.75f) {
						Debug.Log("The " + owner.championName + " refuses to use a HEART to attack!");
						continue;
					}
					if (owner.currentHP >= 0.5f * owner.maxHP && child.GetComponent<Card>().cardScriptableObject.cardValue >= 12 && Random.Range(0f, 1f) < 0.75f) {
						Debug.Log("The opponent is confident! They refuse to use a value of " + child.GetComponent<Card>().cardScriptableObject.cardValue + " to attack!");
						continue;
					}
					if (value < child.GetComponent<Card>().cardScriptableObject.cardValue) {
						selectedCard = child.GetComponent<Card>();
						value = selectedCard.cardScriptableObject.cardValue;
					}
					break;
			}
		}

		if (selectedCard == null) {
			Debug.LogWarning("No card within criteria was found! Returning a null.");
			return null;
		}
		return selectedCard;
	}

	// Deal Functions
	/// <summary>
	/// Deal a specific card to this hand.
	/// </summary>
	/// <param name="specificCard"></param>
	public void DealSpecificCard(CardScriptableObject cardScriptableObject) {
		StartCoroutine(Deal(cardScriptableObject, false, true));
	}
	
	/// <summary>
	/// Deals an amount of randomly generated cards to this hand, with additional parameters for animation and fine control.
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="flip"></param>
	/// <param name="animate"></param>
	/// <param name="abilityCheck"></param>
	/// <returns></returns>
	public IEnumerator Deal(int amount = 4, bool flip = false, bool animate = true, bool abilityCheck = true) {
		for (var i = 0; i < amount; i++) {
			AudioController.instance.Play("Flip");
			// Creates a new card.
			var card = Instantiate(GameController.instance.cardTemplate, Vector2.zero, Quaternion.identity).GetComponent<Card>();
			card.cardScriptableObject = GameController.instance.cardIndex.PlayingCards[Random.Range(0, GameController.instance.cardIndex.PlayingCards.Count)];

			// Noob mode crutch
			if (owner.isPlayer && Random.Range(0f, 1f) < 0.25f) {
				switch (GameController.instance.difficulty) {
					case GameController.Difficulty.Noob:
						int rerollValue = Mathf.Min(card.cardScriptableObject.cardValue + 3, 13);
						foreach (var newCard in GameController.instance.cardIndex.PlayingCards) {
							if (rerollValue != newCard.cardValue || card.cardScriptableObject.cardSuit != newCard.cardSuit || card == newCard) continue;

							Destroy(card.gameObject);
							card = Instantiate(GameController.instance.cardTemplate, Vector2.zero, Quaternion.identity).GetComponent<Card>();
							card.cardScriptableObject = newCard;
						}
						Debug.Log("oh yes crutch");
						break;
				}
			}

			// Sets card to the hand and adds it to the list of cards of this hand for easy reference.
			card.transform.SetParent(transform, false);
			card.owner = owner;
			card.owner.GetMatchStatistic().totalCardsDealt++;
			cards.Add(card);
		
			// Extra parameters.
			if (flip) card.Flip();
			if (animate) {
				card.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
				StartCoroutine(card.GetComponent<SmartHover>().ScaleDown(new Vector3(1f, 1f, 1f)));
			}
			if (abilityCheck) {
				foreach (var selectedChampion in GameController.instance.champions) {
					foreach (var ability in selectedChampion.abilities) {
						yield return StartCoroutine(ability.OnDeal(card, owner));
					}
				}
			}

			yield return new WaitForSeconds(0.25f);
		}
	}
	private IEnumerator Deal(CardScriptableObject cardScriptableObject, bool flip, bool animate, bool abilityCheck = true) {
		var card = Instantiate(GameController.instance.cardTemplate, new Vector2(0, 0), Quaternion.identity).GetComponent<Card>();
		card.cardScriptableObject = cardScriptableObject;
		card.transform.SetParent(transform, false);
		card.owner = owner;
		cards.Add(card);
		
		if (flip) card.Flip();
		if (animate) {
			card.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			StartCoroutine(card.GetComponent<SmartHover>().ScaleDown(new Vector3(1f, 1f, 1f)));
		}

		if (abilityCheck) {
			foreach (var selectedChampion in GameController.instance.champions) {
				foreach (var ability in selectedChampion.abilities) {
					yield return StartCoroutine(ability.OnDeal(card, owner));
				}
			}
		}

		yield return new WaitForSeconds(0.25f);
	}
	/// <summary>
	/// Discards a specified card from this hand.
	/// Do note that this hand must own the card to have authority to discard the card. Otherwise, this will throw an error.
	/// </summary>
	/// <param name="card"></param>
	/// <param name="flip"></param>
	/// <param name="animate"></param>
	/// <param name="abilityCheck"></param>
	/// <returns></returns>
	public IEnumerator Discard(Card card, bool flip = false, bool animate = true, bool abilityCheck = true) {
		// Authority Check
		/*if (card != owner.attackingCard || card != owner.defendingCard) {
			if (!cards.Contains(card) || card.owner != owner) {
				Debug.LogError("This hand does not have authority to discard another hand's card!");
				yield break;
			}
		}*/
		
		
		// Sets card to the discard area, removing the card's specified owner, and removing the card from the list of cards from this hand to avoid memory leaks.
		AudioController.instance.Play("Flip");
		card.transform.SetParent(GameController.instance.discardArea.transform, false);
		if (GameController.instance.discardArea.transform.childCount > 8) {
			for (int i = GameController.instance.discardArea.transform.childCount; i > 8; i--) {
				Destroy(GameController.instance.discardArea.transform.GetChild(0).gameObject);
			}
		}
		if (card.owner != null) {
			card.owner.GetMatchStatistic().totalCardsDiscarded++;
			card.caption.text = "Played by " + card.owner.championName;
		}
		card.owner = null;
		cards.Remove(card);

		// Extra parameters.
		if (flip) card.Flip();
		if (animate) {
			card.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			StartCoroutine(card.GetComponent<SmartHover>().ScaleDown(new Vector3(1f, 1f, 1f)));
		}
		if (abilityCheck) {
			foreach (var selectedChampion in GameController.instance.champions) {
				foreach (var ability in selectedChampion.abilities) {
					// ABILITY CHECK HERE
				}
			}
		}
		yield break;
	}
}
