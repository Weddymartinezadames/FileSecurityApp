# FileSecurityApp

Aplicación de consola en C# para aplicar técnicas de seguridad en archivos, inspirada en CyberChef.

## Características

### 🔐 Criptografía
- Cifrado/Descifrado AES-256 en modo CBC
- Gestión configurable de clave e IV
- Opción de prepend/append del IV al resultado

### 🎨 Esteganografía
- Insertar/Extraer texto oculto en archivos usando LSB (Least Significant Bit)
- Soporte para cualquier tipo de archivo binario
- Preservación de la estructura original

### 📝 Ofuscación
- Base64 (Codificar/Decodificar)
- ROT13 (Rotación de 13 posiciones)
- Inversión de texto

## Estructura del Proyecto

```
FileSecurityApp/
├── Program.cs                 # Punto de entrada y menú principal
├── Models/
│   ├── SecurityResult.cs      # Modelo de resultado de operación
│   └── FileData.cs            # Modelo para datos de archivo
├── Services/
│   ├── CryptographyService.cs # Servicios de criptografía AES
│   ├── SteganographyService.cs# Servicios de esteganografía LSB
│   └── ObfuscationService.cs  # Servicios de ofuscación
├── Utils/
│   ├── FileHandler.cs         # Manejo de lectura/escritura de archivos
│   ├── ConsoleHelper.cs       # Utilidades de consola
│   └── ValidationHelper.cs    # Validación de entrada
└── FileSecurityApp.csproj     # Configuración del proyecto
```

## Uso

```bash
dotnet run
```

Sigue las instrucciones del menú interactivo de consola.

## Requisitos

- .NET 6.0 o superior
- Acceso a archivos (lectura/escritura)

## Autor

Weddymartinezadames
