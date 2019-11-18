# Unity EasyAR 3.0.1 Customization

This files overwrite the basic function from EasyAR in order to increase the speed of ImageTarget's initialization and change the orientation of objects inside ImageTargets to be perpendicular to the image.

### Instalation

**1° - Copy the UnityEasyARCustomization folder to your Assets folder in your project**

**2° - Some errors should show in the console, in order to solve then do the following:**
- **a. - Change centerTransform and targetControllers of the original ImageTrackerBehaviour file to public**
- **b. - Change target, targetImage, Start and OnTracking of the original ImageTarget to public**
- **c. - Add virtual to the function UpdateFrame of the original ImageTrackerBehaviour file**
- **d. - Add virtual to the functions Start, OnTracking of the original ImageTarget file**

### Note

I'm just a begginer, I would love to learn more from any issue or pull request you make, specially in this README file
