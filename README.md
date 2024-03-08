# Level Building Sidekick
LBS is an extension designed for Unity Engine, which facilitates developers in creating environments for their video games, thereby contributing to increased productivity. This extension features artificial intelligence assistants in various areas, which suggest modifications based on the fundamental characteristics developed by the user. These suggestions enable developers to explore new possibilities without manually creating these variations, fostering greater creative freedom.

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Usage](#usage)
- [License](#license)
- [Other Projects](#other-projects)

### Introduction
Level Building Sidekick (LBS) is a tool that allows software developers to define the architecture, constraints, graphical elements, and decorative details that will be used in the construction of a level for video games and other areas. This process enables increased team productivity and the quality and diversity of content they can offer to their users. The tool allows artists to upload any assets they have created for their projects, enabling them to compose custom asset sets for different genres, sections, or styles. LBS is a mixed-initiative content generation tool focused on creating content such as levels, population, and missions.

The intelligent assistants automatically suggest options or variations of this content that share common characteristics with what was initially established by developers. Thus, when creating a level proposal, it is easy to have a variety of configuration alternatives thanks to the different artistic resources that the designer can upload and the suggestions given by the intelligent assistants.

Level Building Sidekick runs as an extension of the Unity game engine. This tool adds a series of new customizable windows within the development environment, following the engine's philosophy of working all related disciplines within it and not needing to use multiple tools at once. It allows modifications to be serialized, thus enabling them to be shared with other developers who can then continue modifying them.

The architecture of this tool is oriented towards element composition, allowing LBS to continue expanding both by the development team and its users. Additionally, this approach makes LBS a great research platform, as it allows new AI techniques to be added to the same tool without having to start from scratch, cutting down development times and allowing researchers to focus on relevant areas of their research.

### Installation
LBS, an extension for Unity Engine, follows a standard installation process similar to other Unity extensions. Below are the necessary steps for installation:
* **System Requirements:** This extension is developed to be compatible with Unity versions 2022.3.3f1 or higher.
* **Download the asset package:** To start the installation, it is required to download the asset package [LBS.package](https://www.google.com/). Once downloaded, simply have Unity Engine open and double-click on the downloaded file. The engine will recognize the package and open a window that will facilitate its import.
* **Additional dependencies:** It is important to note that this tool has dependencies with [Newtonsoft Json](https://docs.unity3d.com/2019.4/Documentation/Manual/com.unity.nuget.newtonsoft-json.html). If this is not installed previously, the project may present multiple errors. To solve this, make sure to install it from Unity's Package Manager.

### Usage
To start working, select the ISILab window in Window>ISILab>LevelBuildingSidekick, this will open the main window of the tool with which you can start working. Next, we will provide a series of general descriptions that you can follow to manage the tool:

In the main window, you will find a large working area visualized with a tiling of squares, in this area you can place, move, and remove elements to shape your levels. To the right of this, you will find an internal inspector that will allow you to work with the specific values of the elements to be modified, their base behaviors, and the assistants that will help you work with this tool.

In addition to this, we can find in the vertical bar between these two aforementioned elements 3 general buttons, which will allow showing and hiding 3 main panels to organize the work. The first corresponds to a panel to organize the content of the level by layers, the second corresponds to a panel to work by layers the quests that are created on these levels, and the third corresponds to the panel that will allow us to pass these levels to structures directly usable in unity.

For detailed information on the different layers that can be created, how the assistants work, and how to interact with the tool more specifically, it is recommended to review the "user manual" available in this same repository.

### License
The license for this project is available in the LICENSE file in this repository. For more details on the license terms and conditions, please see [here](LICENSE).

### Other Projects
Thank you for reviewing this project! To discover more interesting projects, we invite you to visit our website [ISILab.cl](https://isilab.utalca.cl/) and explore everything we have to offer.
