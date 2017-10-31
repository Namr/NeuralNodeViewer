using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    Texture2D[] MRITexture;

    public int layer = 0;
    int lastLayer = 0;

    public Transform floor1;
    public Transform floor2;
    // Use this for initialization
    void Start ()
    {
        
        using (BinaryReader reader = new BinaryReader(File.Open("aal_intensity.hdr", FileMode.Open)))
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

        
        Debug.Log("Pixel Width: " + pixelWidth + "pixelHeight: " + pixelHeight + "pixelDepth: " + pixelDepth + "bitsPerPixel: " + bitsPerPixel + "offset: " + offset);
        //Debug.Log("realWidth: " + realWidth + " realHeight: " + realHeight + " realDepth: " + realDepth);
        

        MRIData = new byte[pixelWidth,pixelHeight,pixelDepth];

        using (BinaryReader reader = new BinaryReader(File.Open("aal_intensity.img", FileMode.Open)))
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

        MRITexture = new Texture2D[pixelDepth];

        for (int z = 0; z < pixelDepth; z++)
        {
            MRITexture[z] = new Texture2D(pixelWidth,pixelHeight,TextureFormat.ARGB32,false);
            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    float grayscale = (float)MRIData[x, y, z];
                    grayscale /= 255;
                    MRITexture[z].SetPixel(x, y, new Color(grayscale,grayscale, grayscale, grayscale));
                }
            }
            MRITexture[z].Apply();
        }
        floor1.GetComponent<Renderer>().material.mainTexture = MRITexture[layer];
        floor2.GetComponent<Renderer>().material.mainTexture = MRITexture[layer];
    }
	
	// Update is called once per frame
	void Update ()
    {      
        if(layer != lastLayer)
        {
            floor1.GetComponent<Renderer>().material.mainTexture = MRITexture[layer];
            floor2.GetComponent<Renderer>().material.mainTexture = MRITexture[layer];
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, map(layer, 0, pixelDepth, -51.7f, 72.4f));
        }
        lastLayer = layer;
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}


