﻿using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
	public enum TooltipType
	{
		Tooltip,
		ErrorTooltip,
		CardTooltip
	}

	public static TooltipSystem instance;

	public Tooltip tooltip;
	public FixedTooltip fixedTooltip;
	public CardTooltip cardTooltip;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	public void Show(string body, string header = "")
	{
		tooltip.UpdateTransform();
		tooltip.UpdatePivot();

		tooltip.SetText(body, header);
		tooltip.canvasGroup.alpha = 0f;
		tooltip.delayIDs.Add(LeanTween.alphaCanvas(tooltip.canvasGroup, 1f, 0.3f).setEaseInOutQuart().setOnComplete(() => tooltip.delayIDs.Clear()).uniqueId);
	}

	public void ShowCard(Card card)
	{
		cardTooltip.Setup(card);

		cardTooltip.transform.localScale = new Vector2(0.8f, 0.8f);
		cardTooltip.canvasGroup.alpha = 0f;
		cardTooltip.delayIDs.Add(LeanTween.scale(cardTooltip.rectTransform, Vector2.one, 0.3f).setEaseInOutQuart().uniqueId);
		cardTooltip.delayIDs.Add(LeanTween.alphaCanvas(cardTooltip.canvasGroup, 1f, 0.3f).setEaseInOutQuart().setOnComplete(() => cardTooltip.delayIDs.Clear()).uniqueId);
	}

	public void ShowError(string body, string header = "")
	{
		fixedTooltip.UpdateTransform();
		fixedTooltip.UpdatePivot();

		fixedTooltip.SetText(body, header);
		fixedTooltip.canvasGroup.alpha = 0f;
		LeanTween.alphaCanvas(fixedTooltip.canvasGroup, 1f, 0.25f).setEaseInOutQuart();
	}

	/// <summary>
	///     Hide all active tooltips.
	/// </summary>
	public void Hide()
	{
		tooltip.GetComponent<CanvasGroup>().alpha = 0f;

		fixedTooltip.GetComponent<CanvasGroup>().alpha = 0f;

		cardTooltip.GetComponent<CanvasGroup>().alpha = 0f;
	}

	/// <summary>
	///     Hide a specific type of tooltip.
	/// </summary>
	/// <param name="tooltipType"></param>
	public void Hide(TooltipType tooltipType)
	{
		float fadeOutTime = 0.1f;

		switch (tooltipType)
		{
			case TooltipType.Tooltip:
				if (tooltip.canvasGroup.alpha == 0f) break;
				foreach (int delayID in tooltip.delayIDs)
				{
					LeanTween.cancel(delayID);
				}
				tooltip.delayIDs.Clear();

				tooltip.delayIDs.Add(LeanTween.alphaCanvas(tooltip.canvasGroup, 0f, fadeOutTime).setEaseInOutQuart().setOnComplete(() => tooltip.delayIDs.Clear()).uniqueId);
				break;
			case TooltipType.ErrorTooltip:
				LeanTween.alphaCanvas(fixedTooltip.canvasGroup, 0f, fadeOutTime).setEaseInOutQuart();
				break;
			case TooltipType.CardTooltip:
				if (cardTooltip.canvasGroup.alpha == 0f) break;
				foreach (int delayID in cardTooltip.delayIDs)
				{
					LeanTween.cancel(delayID);
				}
				cardTooltip.delayIDs.Clear();

				cardTooltip.delayIDs.Add(LeanTween.scale(cardTooltip.rectTransform, new Vector2(0.8f, 0.8f), 0.3f).setEaseInOutQuart().uniqueId);
				cardTooltip.delayIDs.Add(LeanTween.alphaCanvas(cardTooltip.canvasGroup, 0f, 0.3f).setEaseInOutQuart().setOnComplete(() => cardTooltip.delayIDs.Clear()).uniqueId);
				break;
		}
	}
}