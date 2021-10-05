# Pasos actualizados para crear un humano virtual

1. Descargar [MakeHuman](http://www.makehumancommunity.org/content/downloads.html "Download stable release").
2. Descargar MakeHuman plugin for blender y MHX2 (blender side) de la página de MakeHuman [Plugins for Blender](http://www.makehumancommunity.org/content/plugins.html "Plugins MakeHuman-Blender").
3. Descargar [Blender](https://www.blender.org/download/ "Download Blender").
4. Descargar [Unity](https://unity.com/download "Download Unity").
5. Clonar el repositorio del proyecto [HVUAN](https://github.com/herrera78/hvuan "Repositorio HVUAN").
6. Crear un modelo en MakeHuman según las necesidades, añadiendo siempre una lengua y un Rig preset tipo Game engine. Exportar el modelo como .mhx2.
7. En Blender, instalar los plugins previamente descargados, dirigiéndose a la sección Edit -> Preferences -> Add-ons. Al presionar instalar, se busca la carpeta donde se guardaron los plugins en formato .zip y se confirma la instalación.
8. Dentro de Preferences, dirigirse al apartado Save & Load y seleccionar Auto Run Python Scripts. Para guardar las configuraciones presionar Save Preferences.
9. Dirigirse a la carpeta de instalación de los plugins de blender (usualmente la ruta C:\Users\%user%\AppData\Roaming\BlenderFoundation\Blender\2.93\scripts\addons\import_runtime_mhx2\data\hm8\faceshapes) y reemplazar el archivo visemes.mxa por el archivo visemes.mxa del repositorio clonado.
10. Reiniciar Blender.
11. Importar el .mhx2 a Blender desde el apartado File -> Import -> MakeHuman(.mhx2) y seleccionar el modelo exportado desde MakeHuman, posteriormente, marcar la opción Face Shapes y Face Shape Drivers y confirmar. #En está parte podemos ahorrarnos la parte de hacer las vocales como la o y esas cosas si al importar el archivo no seleccionamos FaceShapeDrivers
12. En caso de tener problemas al importar las texturas antes que nada poner el blender en modo de texturas presionando la tecla z y moviendo el cursor a la opción de material preview 
![image](https://user-images.githubusercontent.com/44117920/135951829-122bcb4d-7591-4774-bd30-f6d148af2f8c.png)
 luego de esto si se tiene algun problema con las texturas se va a el modo de shading de blender se selecciona el material con el que haya problema y en el modulo de image as textura, cargamos la textura que nos exporta makehuman al momento de exportar el archivo.
13. En el apartado de Scene Collection en el editor type: outliner seleccionar el componente Body ("nombre_archivo":Body) del personaje importado, y seleccionar el triángulo invertido de color verde dentro de las opciones desplegadas.
14. Dentro del editor type: Properties, buscar en la parte izquierda el triangulo invertido de color verde. 
15. Con el paso 13 y 14 realizado correctamente se habilita la opción Visemes dentro de la pestaña MHX2 Runtime, para cada uno de los Visemes (PP, FF, TH, etc.) se debe añadir un Shape Key con el nombre correspondiente a cada uno de los componentes de Visemes oprimiendo en el botón "+" en la pestaña Shape Keys del editor type: Properties. Este paso 15 se detalla a profundidad en el [Readme](https://github.com/herrera78/VAIFUAN/blob/master/README.md) de este repositorio.
16. Una vez se ha completado el paso 15 para cada uno de los Visemes, se exporta el proyecto como .fbx, para exportar las texturas al momento de la exportación se debe seleccionar en blender el path mode en copy como se muestra en la imagen
![image](https://user-images.githubusercontent.com/44117920/135952529-aab94829-cdeb-4469-af55-6b9839bed011.png)
Luego presionar el boton de embed textures, que se encuentra al lado de la opción que acabamos de modificar
17. Abrir como un proyecto en Unity el repositorio clonado en el paso 5.
18. Para importar nuestro humano FBX de que exportamos desde blender, lo que debemos hacer ahora es crear una nueva carpeta donde vamos a importar nuestro humano virtual, y solamente arrastramos el FBX hacía unity. Este archivo nos aparecerá sin textura alguna, así que debemos seleccionarlo y en la parte de materials damos click a extract textures y creamos o seleccionamos la carpeta donde estas texturas se extraeran. y así mismo con los materiales
