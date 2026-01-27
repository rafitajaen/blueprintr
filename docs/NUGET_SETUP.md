# NuGet Publishing Setup

Para que el workflow publique automáticamente a NuGet, necesitas configurar la autenticación.

## Opción 1: API Key (Recomendado para empezar)

### Paso 1: Obtener API Key

1. Ve a https://www.nuget.org/account/apikeys
2. Si no tienes cuenta, créala primero
3. Click en "Create"
4. Configuración recomendada:
   - **Key Name**: `GitHub Actions - blueprintr`
   - **Glob Pattern**: `Blueprintr*`
   - **Expiration**: 365 días (o la que prefieras)
   - **Scopes**: ✅ Push, ✅ Push new packages and package versions

5. Click "Create" y **copia el API key** (solo se muestra una vez)

### Paso 2: Configurar en GitHub

1. Ve a tu repositorio: https://github.com/rafitajaen/blueprintr
2. Settings → Secrets and variables → Actions
3. Click "New repository secret"
4. Configuración:
   - **Name**: `NUGET_API_KEY`
   - **Value**: [pega el API key que copiaste]
5. Click "Add secret"

### Paso 3: Verificar

Haz un pequeño cambio en `src/Blueprintr/` y push a `main`. El workflow debería publicar automáticamente.

## Opción 2: Trusted Publishing (Más Seguro, Más Complejo)

Trusted Publishing usa OIDC tokens de corta duración en lugar de API keys de larga duración.

### Configurar en NuGet.org

1. Ve a https://www.nuget.org/account/
2. Click en tu paquete (después de haberlo publicado manualmente la primera vez)
3. En "Package owners" → "Trusted publishers"
4. Click "Add trusted publisher"
5. Configuración:
   - **Service**: GitHub Actions
   - **Repository owner**: `rafitajaen`
   - **Repository name**: `blueprintr`
   - **Workflow**: `deploy.yml`

6. También necesitas configurar el secret `NUGET_USERNAME`:
   - GitHub Settings → Secrets → Actions
   - Name: `NUGET_USERNAME`
   - Value: tu username de NuGet.org (NO tu email)

### Primera Publicación Manual (Solo para Trusted Publishing)

Si usas Trusted Publishing, necesitas publicar manualmente la primera versión:

```bash
# Empaqueta el proyecto
dotnet pack src/Blueprintr/Blueprintr.csproj \
  --configuration Release \
  --output ./nupkgs

# Publica a NuGet
dotnet nuget push ./nupkgs/Blueprintr.0.1.0.nupkg \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

Después de esto, el workflow podrá publicar automáticamente con Trusted Publishing.

## Verificar que Funciona

Después de configurar cualquiera de las dos opciones:

1. Haz un cambio en `src/Blueprintr/`
2. Commit y push (o crea un tag)
3. Ve a: https://github.com/rafitajaen/blueprintr/actions
4. Verifica que el job "Publish to NuGet" se ejecute exitosamente
5. Verifica en https://www.nuget.org/packages/Blueprintr/

## Troubleshooting

### Error: "No NuGet authentication configured"
- Verifica que el secret `NUGET_API_KEY` esté configurado correctamente
- O configura Trusted Publishing y el secret `NUGET_USERNAME`

### Error: "Unauthorized"
- API key expirada o inválida
- Regenera el API key en NuGet.org
- Actualiza el secret en GitHub

### Error: "Package already exists"
- Ya existe esa versión en NuGet
- Incrementa la versión creando un nuevo tag git (ej: `git tag 0.1.1`)

## Documentación Oficial

- [NuGet API Keys](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package#create-api-keys)
- [Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
- [GitHub Encrypted Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
