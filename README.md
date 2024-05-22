## How to create your own locations for New Heights

### !Warning! The project doesn't work when you download this repository as zip, because Github will omit dll files. Please clone the project, or use the [Download Link](https://newheightsgame.com/downloads/new-heights-location-tool.zip).

### Note: Steam Workshop features are only available in the Beta version New Heights! The update will be available in the normal version of the game at the end of June 2024.

You can now create your own locations for New Heights and upload them to the steam workshop for the world to try! In this guide we will walk you through the steps required to make your own model, upload it to our steam workshop and climb it in New Heights.

**For creating your own locations you will require some knowledge about the game engine Unity.** There’s no need to code, but you will need to do various actions inside the Unity Editor. We will create an instruction video soon in which we cover most of the basics that you need to create your locations within Unity.

## Step 1 - Download the tools

[Download](https://newheightsgame.com/downloads/new-heights-location-tool.zip) (~124mb) or [clone](https://github.com/Wikkl-Works/new-heights-location-tool) the New heights workshop tool and open it in [Unity 2022.3.10f](unityhub://2022.3.10f1/ff3792e53c62).

If you don’t have Unity yet, download the [Unity Hub](https://unity.com/download) and install Unity 2022.3.10f1 from the [archive](https://unity.com/releases/editor/archive) (In the installer, you don’t need to install Visual Studio, if you’re not planning to code)

## Step 2 - Make a location

Open the Main.unity scene. This is the starting point for creating your own location. By default we have already added:

- A flat, textureless terrain
- A spawn point
- A camera that defines the overview camera point

### Load your models and textures

You can load any models into unity and start building your scene. You can make anything you like, you can delete the terrain we added for you and make your own or simply alter the existing terrain to your liking.

Keep in mind that the scene you build here will be loaded into New Heights as-is. If you don’t have a ground plane in your scene, there won’t be a ground plane in the game. If you don’t have a light in your scene, there won’t be a light in the game. If you add a picture of your granny it will be loaded into the game.

### Asset packs

We don’t have the license to give you access to all of our source materials, like for example terrain textures. But there is the [Unity Assets Store](https://assetstore.unity.com/): and it has all kinds of great asset packs, both paid and free. For most extra textures and vegetation assets we have used assets from [NatureManufacture](https://assetstore.unity.com/publishers/6887).

### Scripting

By creating this location, you are actually creating a ‘mod’ for New Heights. The mod framework also allows for creating new C# scripts that can be exported to the game. It is limited though - because you won’t be able to access functions from New Heights.

## Step 3 - Make your location climbable

There are a couple of requirements for your scene in order to make it work in New Heights:

1. First of all - your models need to have read/write enabled. That way New Heights can actually access the model data in-game. You can change this in the import settings.

2. Back in your scene: your models must have a MeshCollider component (without ‘Convex’ or ‘isTrigger’ checked).

3. Any climbable models must have the Climbable layer.

4. Any models you can use for support but are not very detailed (e.g. terrain, walls, flat surfaces) must have the BaseClimbable layer.

5. There must be at least one object that will determine the player’s spawn position with the Respawn tag. We have put such a spawn point in the default scene.

6. In order to control the menu’s viewport when a player enters your location you add a GameObject with the OverviewCameraPoint tag. The position and rotation of this GameObject will be used as the viewpoint for the menu camera. In the default scene, the ‘Main Camera’ object has this tag.

## Step 4 - Export the location to New Heights

Once you’re happy with your scene it’s time to export it to the Steam Workshop! Start by opening the Steam Workshop wizard by navigating to **New Heights > Steam Workshop Wizard**.

1. Make sure steam is running before continuing, otherwise the wizard won’t work!
2. Make sure that your **active scene** (the currently opened scene in your editor) is the scene you wan to export. The tool will **only** export this scene and its dependencies.

3. If this is your first Steam Workshop item ever created, congratulations! But before you can upload the location, you’ll first have to accept the [Steam Workshop Agreement](https://steamcommunity.com/workshop/workshoplegalagreement/).
4. If this is a new location, click the **Create Item** button to create a new workshop entry.
5. If you’re editing an existing item - please fill in its number / id, so Steam will know which of your items you are updating.
6. Click Export Mod To Game Folder to pack the contents of the scene and save the package locally on your PC. The contents get written to `%AppData%/../LocalLow/Wikkl Works/New Heights/Mods`

After you have pressed **Export Item To Game Folder** the location becomes available for testing in New Heights. Though it’s not yet available online on the Workshop!

## Step 5 - Test your location in New Heights

_Note: You could keep the workshop tool open as well, because you still haven't uploaded your location_

In order to try out your location, you need to launch New Heights. In the game, navigate to the **Community Locations** page where you can find all locations you have subscribed to in the steam workshop and those you have saved locally. Here you can test your location before uploading it to the steam workshop.

## Step 6 - Publish and add routes

### Upload to workshop, edit title & description

1. Select a cover image. You could create one with a screen capture for example. Note: the steam workshop page image has 16:9 dimensions, and the grid/list overview makes them square.
2. Write your change notes. For example "First version" or "Added new Boulder".
3. When you are happy with your location, you can press **Upload Item To Workshop** in the Steam Workshop Wizard to upload the location to the steam workshop.

Your location and its contents are now uploaded! Press **Open URL** to navigate to the item’s workshop page. There you can edit the title, description and fill in the rest of the page of your newly created workshop item.

### Add routes

You can add routes to your location by using the in-game route planning tool. Other people will also be able to create routes in your location! For a more in-depth explanation of how this works check out our [guide](https://newheightsgame.com/blog/how-to-create-your-own-routes).

### Publish

Once you’re ready to release your content into the world, change the visibility of your workshop item to public and you’re done! People will now be able to subscribe to your workshop content and play your location in New Heights!

That’s it! You now know how to create your own location and add it to New Heights for the entire community to use. Now it’s up to you to make your climbing ideas come to life. Good luck, can’t wait to see what you come up with!

## Step 7 - Share & Improve

Do you want to tell people in the community about what you made? Or do you want to know more details? Come join the [Discord Server](https://discord.gg/2DqPft9tnk)!

## Acknowledgments

This tool relies heavily on the awesome [ModTool](https://github.com/Hello-Meow/ModTool) written by [Hello-Meow](https://github.com/Hello-Meow)!
