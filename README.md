# Chat de IAs

Es una aplicación de escritorio que muestra un chat entre dos modelos de IAs.

## Características

- Interfaz de chat con dos participantes (You y Me)
- Integración con LMStudio para la ejecución de modelos de lenguaje
- Animaciones suaves para el desplazamiento de la conversación
- Alternancia automática entre dos modelos de IA diferentes

## Tecnologías utilizadas

- C# / .NET Framework
- WPF (Windows Presentation Foundation)
- RestSharp para llamadas a API
- Newtonsoft.Json para manejo de JSON

## Modelos de IA utilizados

- QuantFactory/Meta-Llama-3-8B-Instruct-GGUF
- TheBloke/dolphin-2.2.1-mistral-7B-GGUF

## Requisitos previos

- LMStudio instalado y configurado en tu sistema
- .NET Framework (versión compatible con el proyecto)

## Configuración

1. Asegúrate de que LMStudio esté ejecutándose y escuchando en `http://localhost:1234`
2. Los modelos mencionados deben estar cargados en LMStudio

## Uso

1. Ejecuta la aplicación
2. Haz clic en el botón "Start" para iniciar una conversación
3. La aplicación alternará automáticamente entre los dos modelos de IA para generar respuestas

## Estructura del proyecto

- `MainWindow.xaml.cs`: Contiene la lógica principal de la interfaz y la interacción con los modelos
- `LMApi.cs`: Maneja las llamadas a la API de LMStudio
- `Message.cs`: Define la estructura de los mensajes en la conversación
- `MessageCollection.cs`: Colección observable de mensajes
- `ConversationView.xaml`: Define la vista de la conversación

## Funcionamiento

La aplicación se comunica con LMStudio a través de llamadas API REST. Cuando se inicia una conversación, la aplicación alterna entre los dos modelos configurados, enviando solicitudes a LMStudio para generar respuestas basadas en el contexto de la conversación.

## Personalización

Puedes modificar los modelos utilizados editando las variables `modelLlama` y `model2` en `MainWindow.xaml.cs`. Asegúrate de que los modelos especificados estén disponibles en tu instancia de LMStudio.

## Contribuir

Las contribuciones son bienvenidas. Por favor, abre un issue para discutir cambios mayores antes de hacer un pull request.

## Licencia

Este proyecto está licenciado bajo la Licencia MIT. Ver [LICENCIA](LICENSE.txt) para detalles.

## Contacto

Raúl Muro Morcillo

[Mi perfil de GitHub](https://github.com/raumuro2)
