## News

As of June 10, 2020, Unity has released the Perceptron Package, which is a more streamlined process of this project.
https://blogs.unity3d.com/2020/06/10/use-unitys-perception-tools-to-generate-and-analyze-synthetic-data-at-scale-to-train-your-ml-models/

As of May 1, 2020, Unity publishes a article on their take on synthetic data which is what essentially our project does.
https://blogs.unity3d.com/2020/05/01/synthetic-data-simulating-myriad-possibilities-to-train-robust-machine-learning-models/

## XR-Showdown-2020 - Synthetic data generation
XR Showdown 2020 Presented by ExxonMobil - Team 2

By utilizing video game technology practices and Unity 3D engine, we can synthetically generate training data for machine learning applications. For example, instead of the manual task of capturing photos of the physical real life object, we can artificially create photorealistic scenes within Unity and use a 3D model of the real-life object to capture infinite amount of screenshots to be used as training data.

Not only are we able to artificially capture images and automatically label them but utilizing a 3D model's render bounding box, we can project the bounding box to the screen and get information of the region in the image. Therefore, we can automate the process of region labeling providing the basis for object detection.

Overview process:
![alt text](https://github.com/choang05/synthetic-data-generation/blob/develop/powerpoint_img2.PNG?raw=true)

## Mobile app demonstration

As part of the XR Showdown, we exploited this technique by creating an mobile app that is used to recognize real world objects trained on the synthetic images. We call this app CORE (Capture Object Recognition Engine).

Overview process:
![alt text](https://github.com/choang05/synthetic-data-generation/blob/develop/powerpoint_img1.PNG?raw=true)

##  Early Development
Below demonstrates what our early scene setup looks like. We have a 3D model (soda can in this case) sourrounded by a sphere point cloud that dictates where the camera will take pictures for training data. Below that shows the images that were taken at random.
![](https://i.imgur.com/gY4TWcT.png)
![](https://i.imgur.com/KcCqDxJ.png)
