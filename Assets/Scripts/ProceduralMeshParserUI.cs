using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralMeshParserUI : MonoBehaviour
{
	[DllImport("user32.dll")]
	private static extern void OpenFileDialog();
	[DllImport("user32.dll")]
	private static extern void SaveFileDialog();

	[SerializeField]
	private InputField mCommandsInput = null;
	[SerializeField]
	private InputField mVariablesInput = null;

	[SerializeField]
	private Button mLoadButton;
	[SerializeField]
	private Button mSaveButton;
	[SerializeField]
	private Button mCompileButton;

	private void Awake()
	{
		mCommandsInput.onValueChanged.AddListener((input) => { ParseInput(mCommandsInput.text, mVariablesInput.text); });
		mVariablesInput.onValueChanged.AddListener((input) => { ParseInput(mCommandsInput.text, mVariablesInput.text); });
		mLoadButton.onClick.AddListener(() => { LoadInput(); });
		mSaveButton.onClick.AddListener(() => { SaveInput(); });
		mCompileButton.onClick.AddListener(() => { ParseInput(mCommandsInput.text, mVariablesInput.text); });
	}

	private void ParseInput(string aInput, string aVariables)
	{
		ProceduralMeshParser.ParseInput(aInput, aVariables);
	}

	private void LoadInput()
	{
		System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
		ofd.Filter = "procedural mesh files (*.pmesh)|*.pmesh";
		if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{
			string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
			string result = string.Empty;
			mCommandsInput.text = string.Empty;
			for (int i = 0; i < lines.Length; ++i)
			{
				result += lines[i];
				if (i < lines.Length - 1)
				{
					result += "\n";
				}
			}
			mCommandsInput.text = result;
		}
	}

	private void SaveInput()
	{
		System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
		sfd.Filter = "procedural mesh files (*.pmesh)|*.pmesh";
		if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{
			System.IO.File.WriteAllLines(sfd.FileName, new string[] { mCommandsInput.text });
		}
	}
}
