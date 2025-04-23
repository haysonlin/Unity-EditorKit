<p align="center">
  <a href="README.md">English</a> | <a href="README-zh_tw.md">中文</a>
</p>

<p align="center">
  <img alt="editorkit header" src=".\Images\Header.png">
</p>

<h3 align="center">Unity Editor Toolbox</h3>

<p align="center">
A container for custom Editor tools, allowing you to register your own Editor Tools or use the built-in toolset.
</p>

<br>

# Built-in Toolset

<h3>1. SpritePacker Quick Switch</h3>
<img alt="spritepacker tool" src=".\Images\CompCapture_SpritePacker.png">

<p>Options:</p>
<ul>
<li>Disable: Disable SpritePacker</li>
<li>V1: Enable SpritePacker V1</li>
<li>V2: Enable SpritePacker V2</li>
</ul>

<h3>2. TimeScale Quick Switch</h3>
<img alt="spritepacker tool" src=".\Images\CompCapture_TimeScale.png">

<span>Click on the numbers to quickly set TimeScale,</span>
<br>
<span>Use [Manual] to set the desired TimeScale in the input field and click [Apply],</span>
<br>
<span>[Set previous value] is used to quickly switch to the previous setting.</span>

<h3>3. FPS Quick Switch (Proposed by 李育杰)</h3>
<img alt="fps switcher" src=".\Images\CompCapture_FPS.png">

<span>Click on the numbers to quickly set the Target frame rate,</span>
<br>
<span>Use [Manual] to set the desired FrameRate in the input field and click [Apply],</span>
<br>
<span>[Set previous value] is used to quickly switch to the previous setting.</span>

<h3>4. CodeEditor Management Tool</h3>
<img alt="codeeditor tool" src=".\Images\CompCapture_CodeEditor.png">

<span>Use the menu to select the CodeEditor to use,</span>
<br>
<span>Click [Open C# Project] to directly open the project with the set CodeEditor,</span>
<br>
<span>Click [Reload Domain] to refresh the script status. Use this if you encounter script editing without triggering compilation.</span>

<h3>5. GameView Screenshot Tool (Created by 李家駿)</h3>
<img alt="game view screenshot tool" src=".\Images\CompCapture_GameViewScreenShot.png">

<span>Click [Select] to choose the folder path,</span>
<br>
<span>Click [Execute] to take a screenshot of the current Game window and save it to the specified folder path.</span>

# Installation

1.  Using UPM Package
    1.  In the Unity Package Manager, use Add package from git URL and enter [`https://github.com/haysonlin/Unity-EditorKit.git?path=src`](https://github.com/haysonlin/Unity-EditorKit.git?path=src)