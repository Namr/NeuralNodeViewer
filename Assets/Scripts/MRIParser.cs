﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0219
#pragma warning disable 0414

public class MRIParser : MonoBehaviour
{
    int pixelWidth;
    int pixelHeight;
    int pixelDepth;
    int bitsPerPixel;
    float offset;

    float realWidth;
    float realHeight;
    float realDepth;

    byte[,,] MRIData;

    Texture2D[] MRIFloorTexture;
    Texture2D[] MRISideTexture;
    Texture2D[] MRIBackTexture;


    Vector3 origin;

    public int Floorlayer = 0;
    public int Sidelayer = 0;
    public int Backlayer = 0;

    int lastFloorLayer = 0;
    int lastSideLayer = 0;
    public Transform floor1;
    public Transform floor2;

    public Transform side1;
    public Transform side2;

    public Slider MRIFloorSlider;
    public Slider MRISideSlider;

    // Use this for initialization
    void Start ()
    {
        INIParser ini = new INIParser();
        ini.Open("config.ini");

        bool isUsed = int.Parse(ini.ReadValue("MRIData", "HasMRI", "0")) == 0 ? false : true;
        string Headerfilepath = ini.ReadValue("MRIData", "MRIHeaderLoc", "0");
        string Imagefilepath = ini.ReadValue("MRIData", "MRIImageLoc", "0");

        int ox = int.Parse(ini.ReadValue("MRIData", "MRIOriginX", "0"));
        int oy = int.Parse(ini.ReadValue("MRIData", "MRIOriginY", "0"));
        int oz = int.Parse(ini.ReadValue("MRIData", "MRIOriginZ", "0"));

        origin = new Vector3(ox,oy,oz);
        this.transform.localPosition = origin;

        MRISideSlider.transform.parent.gameObject.SetActive(isUsed);
        this.transform.gameObject.SetActive(isUsed);

        using (BinaryReader reader = new BinaryReader(File.Open(Headerfilepath, FileMode.Open)))
        {
            //http://www.grahamwideman.com/gw/brain/analyze/formatdoc.htm
            int sizeof_hdr = reader.ReadInt32();
            char[] data_type = reader.ReadChars(10);
            char[] db_name = reader.ReadChars(18);
            int extents = reader.ReadInt32();
            int session_error = reader.ReadInt16();
            char regular = reader.ReadChar();
            char hkey_un0 = reader.ReadChar();
            int dimNum = reader.ReadInt16();
            int width = reader.ReadInt16();
            int height = reader.ReadInt16();
            int depth = reader.ReadInt16();
            int time = reader.ReadInt16();

            int dim5 = reader.ReadInt16();
            int dim6 = reader.ReadInt16(); //these three are undocumented and do nothing
            int dim7 = reader.ReadInt16();

            char[] vox_units = reader.ReadChars(4);
            char[] cal_units = reader.ReadChars(8);
            int unused1 = reader.ReadInt16();
            int datatype = reader.ReadInt16();
            int bitpix = reader.ReadInt16();
            int dim_un0 = reader.ReadInt16();

            float pixdim0 = reader.ReadSingle();
            float pixdim1 = reader.ReadSingle();
            float pixdim2 = reader.ReadSingle();
            float pixdim3 = reader.ReadSingle();
            float pixdim4 = reader.ReadSingle();
            float pixdim5 = reader.ReadSingle();
            float pixdim6 = reader.ReadSingle();
            float pixdim7 = reader.ReadSingle();

            float vox_offset = reader.ReadSingle();

            pixelWidth = width;
            pixelHeight = height;
            pixelDepth = depth;
            bitsPerPixel = bitpix;
            offset = vox_offset;

            realWidth = pixdim1;
            realHeight = pixdim2;
            realDepth = pixdim3;
        }

        
        //Debug.Log("Pixel Width: " + pixelWidth + "pixelHeight: " + pixelHeight + "pixelDepth: " + pixelDepth + "bitsPerPixel: " + bitsPerPixel + "offset: " + offset);
        //Debug.Log("realWidth: " + realWidth + " realHeight: " + realHeight + " realDepth: " + realDepth);
        

        MRIData = new byte[pixelWidth,pixelHeight,pixelDepth];

        using (BinaryReader reader = new BinaryReader(File.Open(Imagefilepath, FileMode.Open)))
        {
            for(int z = 0;z < pixelDepth;z++)
            {
                for (int y = 0; y < pixelHeight; y++)
                {
                    for (int x = 0; x < pixelWidth; x++)
                    {
                        MRIData[x, y, z] = reader.ReadByte();
                    }
                }
            }
        }

        MRIFloorTexture = new Texture2D[pixelDepth];

        for (int z = 0; z < pixelDepth; z++)
        {
            MRIFloorTexture[z] = new Texture2D(pixelWidth,pixelHeight,TextureFormat.ARGB32,false);
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    float grayscale = (float)MRIData[x, y, z];
                    grayscale /= 255;
                    float transparency;
                    if(grayscale > 0.1f)
                    {
                        transparency = 1;
                    }
                    else
                    {
                        transparency = 0;
                    }
                    MRIFloorTexture[z].SetPixel(x, y, new Color(grayscale, grayscale, grayscale, transparency));
                }
            }
            MRIFloorTexture[z].Apply();
        }


        MRISideTexture = new Texture2D[pixelHeight];

        for (int y = 0; y < pixelHeight; y++)
        {
            MRISideTexture[y] = new Texture2D(pixelWidth, pixelDepth, TextureFormat.ARGB32, false);
            for (int z = 0; z < pixelDepth; z++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    float grayscale = (float)MRIData[x, y, z];
                    grayscale /= 255;
                    float transparency;
                    if (grayscale > 0.1f)
                    {
                        transparency = 1;
                    }
                    else
                    {
                        transparency = 0;
                    }
                    MRISideTexture[y].SetPixel(x, z, new Color(grayscale, grayscale, grayscale, transparency));
                }
            }
            MRISideTexture[y].Apply();
        }

        floor1.transform.localScale = new Vector3(pixelWidth,1, pixelHeight);
        floor1.GetComponent<Renderer>().material.SetFloat("_Mode",2.0f);
        floor1.GetComponent<Renderer>().material.mainTexture = MRIFloorTexture[Floorlayer];

        floor2.transform.localScale = new Vector3(pixelWidth, 1, pixelHeight);
        floor2.GetComponent<Renderer>().material.SetFloat("_Mode", 2.0f);
        floor2.GetComponent<Renderer>().material.mainTexture = MRIFloorTexture[Floorlayer];

        side1.transform.localScale = new Vector3(pixelWidth, 1, pixelDepth);
        side1.GetComponent<Renderer>().material.SetFloat("_Mode", 2.0f);
        side1.GetComponent<Renderer>().material.mainTexture = MRISideTexture[Floorlayer];

        side2.transform.localScale = new Vector3(pixelWidth, 1, pixelDepth);
        side2.GetComponent<Renderer>().material.SetFloat("_Mode", 2.0f);
        side2.GetComponent<Renderer>().material.mainTexture = MRISideTexture[Floorlayer];

        if (MRIFloorSlider != null && MRISideSlider != null)
        {
            MRIFloorSlider.maxValue = pixelDepth - 1;
            MRIFloorSlider.minValue = 0;

            MRISideSlider.maxValue = pixelHeight - 1;
            MRISideSlider.minValue = 0;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {      
        if(MRIFloorSlider != null && MRISideSlider != null)
        {
            Floorlayer = (int)MRIFloorSlider.value;
            Sidelayer = (int)MRISideSlider.value;
        }

        if(Floorlayer != lastFloorLayer || Sidelayer != lastSideLayer)
        {
            floor1.GetComponent<Renderer>().material.mainTexture = MRIFloorTexture[Floorlayer];
            floor2.GetComponent<Renderer>().material.mainTexture = MRIFloorTexture[Floorlayer];

            side1.GetComponent<Renderer>().material.mainTexture = MRISideTexture[Sidelayer];
            side2.GetComponent<Renderer>().material.mainTexture = MRISideTexture[Sidelayer];

            floor1.localPosition = new Vector3(floor1.localPosition.x, floor1.localPosition.y, map(Floorlayer, 0, pixelDepth, -51.7f, 72.4f) / transform.localScale.z);
            floor2.localPosition = new Vector3(floor2.localPosition.x, floor2.localPosition.y, (map(Floorlayer, 0, pixelDepth, -51.7f, 72.4f) / transform.localScale.z) + 0.09f);

            side1.localPosition = new Vector3(side1.localPosition.x, map(Sidelayer, 0, pixelHeight, -81.99f, 54.06f) / transform.localScale.y, side1.localPosition.z);
            side2.localPosition = new Vector3(side1.localPosition.x, (map(Sidelayer, 0, pixelHeight, -81.99f, 54.06f) / transform.localScale.y) + 0.09f, side1.localPosition.z);
        }
        lastFloorLayer = Floorlayer;
        lastSideLayer = Sidelayer;
    }

    public void ToggleEnabled()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}

