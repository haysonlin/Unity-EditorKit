<p align="center">
  <a href="README.md">English</a> | <a href="README-zh_tw.md">中文</a>
</p>

<p align="center">
  <img alt="editorkit header" src=".\Images\Header.png">
</p>

<h3 align="center">Unity 編輯器工具箱</h3>

<p align="center">
一個用來裝自訂Editor工具的容器，可以自行將自製的Editor Tool註冊到容器中，或使用內建的工具集。
</p>

<br>

# 內建的工具集

<h3>1. SpritePacker 快捷切換</h3>
<img alt="spritepacker tool" src=".\Images\CompCapture_SpritePacker.png">

<p>選項 :</p>
<ul>
<li>Disable : 禁用 SpritePacker</li>
<li>V1 : 啟用 SpritePacker V1</li>
<li>V2 : 啟用 SpritePacker V2</li>
</ul>

<h3>2. TimeScale 快捷切換</h3>
<img alt="spritepacker tool" src=".\Images\CompCapture_TimeScale.png">

<span>點擊數字可以快速設定TimeScale,</span>
<br>
<span>透過 [Manual] 可以在輸入框設定想要的TimeScale然後點擊 [Apply] 套用,</span>
<br>
<span>[Set previous value] 用來快速切換前一個設定值.</span>

<h3>3. FPS 快捷切換 (由李育杰提出)</h3>
<img alt="fps switcher" src=".\Images\CompCapture_FPS.png">

<span>點擊數字可以快速設定Target frame rate,</span>
<br>
<span>透過 [Manual] 可以在輸入框設定想要的FrameRate然後點擊 [Apply] 套用,</span>
<br>
<span>[Set previous value] 用來快速切換前一個設定值.</span>

<h3>4. CodeEditor 管理工具</h3>
<img alt="codeeditor tool" src=".\Images\CompCapture_CodeEditor.png">

<span>透過選單可以選擇要使用的CodeEditor,</span>
<br>
<span>點擊 [Open C# Project] 可以直接透過設定的 CodeEditor 開啟專案,</span>
<br>
<span>點擊 [Reload Domain] 可以刷新腳本狀態, 如果遇到編輯腳本沒有觸發編譯的話可以使用.</span>

<h3>5. GameView 螢幕截圖工具 (由李家駿製作)</h3>
<img alt="game view screenshot tool" src=".\Images\CompCapture_GameViewScreenShot.png">

<span>點擊 [Select] 選擇資料夾路徑,</span>
<br>
<span>點擊 [Execute] 會將當前的Game視窗的畫面截圖並存在所設定的資料夾路徑.</span>
<br>


# 安裝方式

1. 使用 UPM Package
    1. 在 Unity Package Manager 中透過 Add package from git URL 帶入 [`https://github.com/haysonlin/Unity-EditorKit.git?path=src`](https://github.com/haysonlin/Unity-EditorKit.git?path=src)