using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleClockQuizManager : SingletonMonobehaviour<CircleClockQuizManager>
{
    [SerializeField] GameObject QuizParent;

    // List<List> Image がなぜかInspectorに表示されないので仕方なく全部定義する
    [SerializeField] List<Image> ZeroImages;
    [SerializeField] List<Image> OneImages;
    [SerializeField] List<Image> TwoImages;
    [SerializeField] List<Image> ThreeImages;
    [SerializeField] List<Image> FourImages;
    [SerializeField] List<Image> FiveImages;
    [SerializeField] List<Image> SixImages;
    [SerializeField] List<Image> SevenImages;
    [SerializeField] List<Image> EightImages;
    [SerializeField] List<Image> NineImages;

    [SerializeField] private List<Image> allImages;

    [SerializeField] Transform TimerTextTrans;
    private Vector3 initTimerTextPos;

    private void Start()
    {
        AddAllImages();
        initTimerTextPos = TimerTextTrans.position;
    }

    public bool IsDisplayed()
    {
        return QuizParent.activeSelf;
    }

    public void DisplayQuiz()
    {
        QuizParent.SetActive(true);
        MoveTimerUI();        
    }

    public void DisableQuiz()
    {
        QuizParent.SetActive(false);
        MoveTimerUI();
    }

    public void DisplayCircleNumber(List<int> displayNumbers)
    {
        AllNumbersDisable();
        foreach(int num in displayNumbers)
        {
            DisplayCircleNumber(num);
        }
    }

    private void MoveTimerUI()
    {
        if(IsDisplayed())
        {
            TimerTextTrans.position = new Vector3(0, 200, 0);
        }
        else
        {
            TimerTextTrans.position = initTimerTextPos;
        }
    }

    private void DisplayCircleNumber(int num)
    {
        switch(num)
        {
            case 0:
                NumberImagesListSetActive(ZeroImages, false);
                break;
            case 1:
                NumberImagesListSetActive(OneImages, false);
                break;
            case 2:
                NumberImagesListSetActive(TwoImages, false);
                break;
            case 3:
                NumberImagesListSetActive(ThreeImages, false);
                break;
            case 4:
                NumberImagesListSetActive(FourImages, false);
                break;
            case 5:
                NumberImagesListSetActive(FiveImages, false);
                break;
            case 6:
                NumberImagesListSetActive(SixImages, false);
                break;
            case 7:
                NumberImagesListSetActive(SevenImages, false);
                break;
            case 8:
                NumberImagesListSetActive(EightImages, false);
                break;
            case 9:
                NumberImagesListSetActive(NineImages, false);
                break;
        }
    }

    private void NumberImagesListSetActive(List<Image> images, bool active)
    {
        if(images.Count == 0) { return; }
        foreach(Image image in images)
        {
            image.enabled = active;
        }
    }

    private void AllNumbersDisable()
    {
        NumberImagesListSetActive(allImages, true);
    }

    private void AddAllImages()
    {
        AddToAllImage(ZeroImages);
        AddToAllImage(OneImages);
        AddToAllImage(TwoImages);
        AddToAllImage(ThreeImages);
        AddToAllImage(FourImages);
        AddToAllImage(FiveImages);
        AddToAllImage(SixImages);
        AddToAllImage(SevenImages);
        AddToAllImage(EightImages);
        AddToAllImage(NineImages);
    }

    private void AddToAllImage(List<Image> images)
    {
        if(images.Count == 0)
        {
            return;
        }
        foreach(Image image in images)
        {
            allImages.Add(image);
        }
    }
}
