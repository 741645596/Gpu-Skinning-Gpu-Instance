using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGGUtil  {

	// 获取n最接近的2 幂次
	public static int GettoPower(int n)
	{
		// 一个通道一个像素。
		int npower = 2;
		while (n > npower) {
			npower = npower * 2;
		}

		return npower;
	}

	// RGBM编码
	public static float MaxRange = 10;
	// 通过编码值保证rgb在0——1 的范围内
	public static Vector3 offset = new Vector3(3.5f, 0.0f, 1.5f);
	public static Color EncodeRGBM(Vector3 pos)
	{
		Vector3 newPos = pos + offset;
		float maxRgb = Mathf.Max (newPos.x, Mathf.Max (newPos.y, newPos.z));
		float M = maxRgb / MaxRange;
		M = Mathf.Ceil (M * 255.0f)/ 255.0f;
		float ms = M * MaxRange;
		return new Color (newPos.x / ms, newPos.y / ms, newPos.z / ms, M);
	}

	// rgbm 解码
	public static Vector3 decodeRGBM(Color c)
	{
		float ms = c.a * MaxRange;
		Vector3 v = new Vector3 (c.r * ms, c.g * ms, c.b * ms);
		return v - offset;
	}


	// 用于编码的参数矩阵 3 * 3
	public static Vector3 LogLuvM1 = new Vector3(0.2209f, 0.3390f, 0.4184f);
	public static Vector3 LogLuvM2 = new Vector3(0.1138f, 0.6780f, 0.7319f);
	public static Vector3 LogLuvM3 = new Vector3(0.0102f, 0.1130f, 0.2969f);

	// 逆矩阵，用来进行解码。
	public static Vector3 LogLuvInverseM1 = new Vector3(6.0014f, -2.708f, -1.7996f);
	public static Vector3 LogLuvInverseM2 = new Vector3(-1.3320f, 3.1029f, -5.7721f);
	public static Vector3 LogLuvInverseM3 = new Vector3(0.3008f,   -1.0882f,  5.6268f);



	// logluv 编码
	public static Color EncodeLogLuv(Vector3 pos)
	{
		Vector3 newPos = pos + offset;
		newPos = newPos / MaxRange;

		Color ret;

		Vector3 v3 = CalcMatrix(LogLuvM1, LogLuvM2, LogLuvM3, newPos);
		v3 = Vector3.Max(v3, new Vector3(0.000001f, 0.000001f, 0.000001f));
		ret.r = v3.x / v3.z;
		ret.g = v3.y / v3.z;
		float le = 2 * Mathf.Log(v3.y) + 127;
		ret.a = le - Mathf.FloorToInt(le);
		ret.b = (le - Mathf.Floor (ret.a * 255.0f) / 255.0f) / 255.0f;
		return ret;
	}


	// logluv 解码
	public static Vector3 DecodeLogLuv(Color c)
	{
		float Le = c.b * 255 + c.a;
		Vector3 v3;
		v3.y = Mathf.Exp ((Le - 127) / 2);
		v3.z = v3.y / c.g;
		v3.x = c.r * c.b;

		Vector3 vv = CalcMatrix(LogLuvInverseM1, LogLuvInverseM2, LogLuvInverseM3, v3);;
		vv = Vector3.Max(vv, Vector3.zero);
		//
		Vector3 newPos = vv * MaxRange;
		newPos = newPos + offset;
		return newPos;
	}


	public static Vector3 CalcMatrix(Vector3 Row1, Vector3 Row2, Vector3 Row3, Vector3 pos)
	{
		Vector3 v = Vector3.zero;
		v.x = CalcMatrix(new Vector3(Row1.x, Row2.x, Row3.x), pos);
		v.y = CalcMatrix(new Vector3(Row1.y, Row2.y, Row3.y), pos);
		v.z = CalcMatrix(new Vector3(Row1.z, Row2.z, Row3.z), pos);
		return v;
	}

	public static float CalcMatrix(Vector3 Row, Vector3 pos)
	{
		return Row.x * pos.x + Row.y * pos.y + Row.z * pos.z;
	}


	public static float framerate = 30f;
}
