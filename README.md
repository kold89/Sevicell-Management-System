# Sevicell Pro

Sistema de gestión comercial de escritorio para negocios de venta de celulares: compras, ventas, **inventario serializado por IMEI** y **contratos de crédito** con cuotas y pagos parciales.

> *English summary:* Sevicell Pro is a WPF/.NET desktop ERP for phone retail businesses. It features IMEI-level serialized inventory, installment credit contracts with partial payments, and role-based access control with audit logging. Built with C#, WPF (MVVM), Entity Framework Core and SQL Server.

**Estado:** en desarrollo activo.


## Qué problema resuelve

Un negocio de celulares necesita saber **qué equipo exacto** (por IMEI) compró, vendió y a quién, y llevar el control de las ventas a crédito: cuotas, abonos y atrasos. Sevicell reúne todo eso en una sola aplicación, con trazabilidad por unidad y permisos por rol.

## Funcionalidades

### Compras y ventas
- Registro de facturas de compra y de venta.
- Reporte diario de ventas.

### Inventario serializado
- Productos marcados como serializados o con stock por cantidad.
- Cada unidad física se registra con IMEI, IMEI2, número de serie, color, modelo y **estado** (Disponible / Vendido).
- Trazabilidad de cada equipo desde la compra hasta la venta.
- Listado de dispositivos con filtro por estado; al abrir un equipo vendido se muestra el detalle de su contrato.

### Contratos de crédito
- Cálculo automático de cuotas **semanales o mensuales**, con tasa de interés editable por contrato.
- Guardado **transaccional**: contrato, cuotas y cambio de estado de la unidad se guardan juntos o no se guardan.
- Pagos totales y **parciales**, con historial de abonos por cuota.
- Listado de contratos con filtros, paginación y detalle de cuotas y pagos.
- Pantalla de cobros con tarjetas de urgencia (vencidas, hoy, esta semana).
- Estados de cuota: `PENDIENTE`, `PAGADO`, `VENCIDO`, `PARCIAL`, `PAGO_TARDE`, `CONDONADO`.

### Seguridad y auditoría
- Control de acceso basado en roles (RBAC) con permisos granulares.
- El menú se actualiza según los permisos del usuario.
- Registro de auditoría de operaciones críticas.

### Experiencia de usuario
- Notificaciones tipo *toast* en lugar de diálogos modales.
- Control de paginación reutilizable.

## Tecnologías

| Tecnología | Uso |
|---|---|
| C# y .NET | Lenguaje y plataforma |
| WPF (MVVM) | Interfaz de escritorio |
| Entity Framework Core y LINQ | Acceso a datos |
| Microsoft SQL Server | Base de datos |
| PowerShell + `sqlcmd` | Script de respaldo |
| Inno Setup | Instalador de distribución |
| Git y GitHub | Control de versiones |

## Estructura del proyecto

Organizado con el patrón **MVVM**:

| Carpeta | Contenido |
|---|---|
| `Views/` | Ventanas y pantallas (XAML) |
| `ViewModels/` | Lógica de presentación de cada vista |
| `Models/` | Entidades del dominio |
| `Data/` | Acceso a datos con Entity Framework Core |
| `Services/` | Reglas de negocio (por ejemplo, el cálculo de cuotas) |
| `Security/` | Roles, permisos y autenticación |
| `ControlsUI/` | Controles reutilizables (paginador, entre otros) |
| `Converters/` | Convertidores de valores de WPF |
| `Plantillas/` | Plantillas de documentos |

## Decisiones de diseño

- **Inventario por unidad:** una fila por equipo físico, ligada a su compra y a su venta, en lugar de un simple contador de stock. Permite auditar garantías y saber qué IMEI se vendió y a quién.
- **Permisos en memoria:** un gestor de permisos mantiene los del usuario en un `HashSet` para consultarlos en O(1) y refresca el menú por eventos al cambiar los roles.
- **Operaciones atómicas:** crear un contrato implica varias tablas y un cambio de estado en la unidad; se hace en una sola transacción.
- **Lógica fuera de la interfaz:** el cálculo de cuotas vive en servicios, no en las ventanas.

## Próximos pasos

- [ ] Dashboard con datos reales (ventas, contratos, cobros e inventario) y gráficos.
- [ ] Transición automática de estados de las cuotas.
- [ ] Pruebas unitarias (xUnit) sobre el cálculo de cuotas y los permisos.
- [ ] Módulo de reparaciones.
- [ ] API web para sincronización entre sucursales.

## Cómo ejecutarlo

**Requisitos**
- Windows 10 u 11
- Visual Studio con la carga de trabajo de escritorio de .NET
- SQL Server (Express funciona) o LocalDB

## Licencia

Distribuido bajo la licencia MIT. Consulta el archivo [LICENSE.txt](LICENSE.txt).