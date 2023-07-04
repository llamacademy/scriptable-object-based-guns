# ScriptableObject-Based Gun System
Learn how to create a ScriptableObject-based gun system from scratch for your game! 

In this tutorial repository and accompanying [video series](https://www.youtube.com/watch?v=E-vIMamyORg&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to), you'll learn how to make a configuration-driven gun system with ScriptableObjects that will be able to have:
* Hitscan Guns - [Implemented in Part 1](https://www.youtube.com/watch?v=E-vIMamyORg&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=1)
* Simple Bullet Spread & Recoil - [Implemented in Part 1](https://www.youtube.com/watch?v=E-vIMamyORg&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=1).
* Simple Procedural Recoil - [Implemented in Part 2](https://www.youtube.com/watch?v=pwq7F5DeQnI&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=2)
* Procedural Recoil based on Texture - [Implemented in Part 2](https://www.youtube.com/watch?v=pwq7F5DeQnI&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=2)
* Simple Impact Damage - [Implemented in Part 3](https://www.youtube.com/watch?v=6yvUmSxlGQo&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=3) with `IDamageable` System for damaging arbitrary objects including enemies.
* Reloading - [Implemented in Part 4](https://www.youtube.com/watch?v=Tn8RYWnEd94&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=4)
* Sound Effects - [Implemented in Part 5](https://www.youtube.com/watch?v=hV3BAw2c9Io&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=5)
* Projectile Guns - [Implemented in Part 6](https://www.youtube.com/watch?v=LIB7uGDZou0&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=6)
* Accurate Aiming / Crosshairs - [Implemented in Part 7](https://www.youtube.com/watch?v=x8ECpNWMmag&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=7&pp=sAQB) + With Inverse Kinematics and Animation Rigging in [Part 12](https://www.youtube.com/watch?v=chgLRjSaoXc&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=12)
* Attachments - [Modifier System in Part 8](https://www.youtube.com/watch?v=RbIk6VnwHnI&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=8) + Attachments UI & Applying to a Gun in [Part 10](https://www.youtube.com/watch?v=8wBEb2l0vZQ&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=10)
* Animated Models - Coming with a Future Video
* Damage Effects such as burning, freezing, explosions, etc... - [Implemented in Part 9](https://www.youtube.com/watch?v=Y-Qr6GPN2v0&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=9) with `ICollisionHandler`
* Gun & Ammo Pick-ups - [Implemented in Part 11](https://www.youtube.com/watch?v=Fpt9xA3Ftmo&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=11)
* And even more! - Coming with a Future Video

Impact Effects _are not_ in scope of this tutorial series. They are handled using the [Surface Manager](https://github.com/llamacademy/surface-manager) (tutorial [video here](https://youtu.be/kT2ZxjMuT_4)).

[![Youtube Tutorial](./Video%20Screenshot.jpg)](https://www.youtube.com/watch?v=E-vIMamyORg&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to)

## Quickstart
If you're following along with the tutorial series, check out the appropriate branch for the video you're watching. `main` will be the latest version and each branch such as `part-1` will be where a particular video ended.

Make sure to import the [Unity Particle Pack](https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-particle-pack-127325) after checking out this repository for all the bullet shooting and impact effects.
You can import just the folder "EffectExamples" and ignore the rest of the files. This will prevent overriding all your project settings.

Most likely you will also need to run the [Render Pipeline Material Converter](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@12.0/manual/features/rp-converter.html) since we're using URP and Unity Particle Pack (1.0-1.7) ships with only Built-In Render Pipeline support.
These particle systems loop, so you may also need to turn off "Looping" on each Particle System in use.

As of April 4, 2023, this uses [Assembly Definitions](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html) ([tutorial](https://youtu.be/qprZHOPu2OI)) to package the files. If you are using Assembly Definition files in your project, you may need to add references to `LlamAcademy.Guns`, `LlamAcademy.ImpactSystem`, and if you choose to use the demo scripts, `LlamAcademy.Guns.Demo`.

You will need to download "Rifle Idle" and "Pistol Idle" animations from [Mixamo](https://mixamo.com/#/) or your preferred animation for those states. The demo Animator assumes these exist already. You may need to hook them up on the "Gun Layer" of the `StarterAssetsThirdPerson.controller`

## Scope of the Project
This repository is primarily intended as a learning tool with the [Gun Series](https://www.youtube.com/watch?v=E-vIMamyORg&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to). However, you can absolutely take this and use it in your game. There are some key limitations to call out as this repository grows and we cover more things that if you're just pulling this repo, you should be aware of.

This repository is focused on Guns and making a flexible gun system to support all kinds of guns. How they work, configuring them, ammo types, this kind of thing. The core functionality of the Guns are housed in `LlamAcademy.Guns` and sub-namespaces. Anything listed as `Demo` was implemented to showcase some functionality, and may not be fully thought through. It can be used for reference implementation but may have holes in functionality and may not work "out of the box" for your game. This includes the Player Controller. A fully functional Player Controller is _not_ (currently) in scope of this repository. I am using the Starter Assets Third Person Controller and doing some basic integrations to show a starting point only. Having different states such as "aiming" is more of a character controller function than strictly the gun system function.

### Pull Requests
If there is something broken on a particular branch (each video has a dedicated branch that ends at the end state of that video), please feel free to open a PR to address that problem.

Since this is intended as a learning tool more than an "off the shelf solution for everything related to guns", requests can be made, but new features will not be added without an accompanying tutorial.

### Aiming
Starting with [Part 12](https://www.youtube.com/watch?v=chgLRjSaoXc&list=PLllNmP7eq6TQJjgKJ6FKcNFfRREe_L6to&index=12), Aiming with the Animation Rigging package was added, and right click to aim in the ThirdPersonController was removed. 

Aiming is handled by clamping the camera rotation to relatively closely match what the player's rig can bend to. It's important to note as well that `ShootType.FromGun` does not work well with Animation Rigging and IK since it has the Gun driving the aim target position, and the IK / Rigging trying to aim the gun. If you want to use "FromGun" the Animation Rigging should be disabled or weights set to 0.

### Escape / Customize Menu
In the demo scene, pressing `Esc` brings up a customization menu. This is likely not realistic. In most games, players cannot not dynamically change modifiers at runtime. If you imagine this happens on a menu before spawning the player, it should work as expected. If you press escape after running around, swapping guns, and shooting, you may not get the result you expected because not all cases were covered.

### Animations
Animations such as Pistol Idle and Rifle Aim cannot be included in the repository. You can find the Pistol Idle on [Mixamo](https://mixamo.com/#/) for free labeled "Pistol Idle". They also have a Rifle Idle animation you can bring in as well.

## Supporters
Have you been getting value out of these tutorials? Do you believe in LlamAcademy's mission of helping everyone make their game dev dream become a reality? Consider becoming a Patreon supporter and get your name added to this list, as well as other cool perks.
Head over to https://patreon.com/llamacademy to show your support.

If you'd prefer to become a [YouTube Member](https://www.youtube.com/channel/UCnWm6pMD38R1E2vCAByGb6w/join) you can get all the same benefits on that platform!

Want to provide one-time support? You can send a Super Thanks on any video!

### Phenomenal Supporter Tier
* Andrew Bowen
* YOUR NAME HERE!

### Tremendous Supporter Tier
* Bruno Bozic
* YOUR NAME HERE!

### Awesome Supporter Tier
* AudemKay
* Matt Parkin
* Ivan
* Reulan
* Iffy Obelus
* Dwarf
* SolarInt
* YOUR NAME HERE!

### Supporters
* Trey Briggs
* Matt Sponholz
* Dr Bash
* Tarik
* Sean
* ag10g
* Elijah Singer
* Lurking Ninja
* Josh Meyer
* Ewald Schulte
* Dom C
* Andrew Allbright
* AudemKay
* Claduiu Barsan-Pipu
* Ben
* Xuul
* Christiaan Van Steenwijk
* YOUR NAME HERE!

### ♥ Super Thanks
* Tom 

## Other Projects
Interested in AI Topics in Unity, or other tutorials on Unity in general? 

* [Check out the LlamAcademy YouTube Channel](https://youtube.com/c/LlamAcademy)!
* [Check out the LlamAcademy GitHub for more projects](https://github.com/llamacademy)

## Socials
* [YouTube](https://youtube.com/c/LlamAcademy)
* [Facebook](https://facebook.com/LlamAcademyOfficial)
* [TikTok](https://www.tiktok.com/@llamacademy)
* [Twitter](https://twitter.com/TheLlamAcademy)
* [Instagram](https://www.instagram.com/llamacademy/)
* [Reddit](https://www.reddit.com/user/LlamAcademyOfficial)

## Requirements
* Requires Unity 2021.3 LTS or higher.
* You will need to download Rifle Idle and Pistol Idle animations from [Mixamo](https://mixamo.com/#/) or your preferred animation for those states.
* [Unity Particle Pack](https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-particle-pack-127325)
* [Surface Manager](https://github.com/llamacademy/surface-manager) - Included in this repository, but not covered in the tutorial series. Tutorial [video here](https://youtu.be/kT2ZxjMuT_4)).