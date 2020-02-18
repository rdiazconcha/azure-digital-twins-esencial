var condition = "condicion";
var temperatureType = "Temperature";
var temperatureThreshold = 24;

function process(telemetry, executionContext) {
    try {

        // Obtiene los metadatos del sensor
        var sensor = getSensorMetadata(telemetry.SensorId);

        // Obtiene la lectura
        var parseReading = JSON.parse(telemetry.Message);

        // Establece el valor del sensor
        setSensorValue(telemetry.SensorId, sensor.DataType, parseReading.SensorValue);

        // Consulta el espacio padre
        var parentSpace = sensor.Space();

        // Obtiene los demás sensores del espacio
        var otherSensors = parentSpace.ChildSensors();

        // Obtiene la temperatura
        var temperatureSensor = otherSensors.find(function (element) {
            return element.DataType === temperatureType;
        });

        // Obtiene los últimos valores
        var temperatureValue = getFloatValue(temperatureSensor.Value().Value);

        // Si no hay valor de temperatura, regresa inmediatamente
        if (temperatureValue === null) {
            sendNotification(telemetry.SensorId, "Sensor", "Error: temperature is null, returning");
            return;
        }

        var alert = "Temperatura apta";
        var noAlert = "Temperatura demasiado alta";

        // Si los valores del sensor están dentro de rango
        if (temperatureValue < temperatureThreshold) {

            //Establece el valor para el espacio
            setSpaceValue(parentSpace.Id, condition, alert);

            // Notifica el espacio
            parentSpace.Notify(JSON.stringify(alert));
        }
        else {
            //Establece el valor para el espacio
            setSpaceValue(parentSpace.Id, condition, noAlert);
        }
    }
    catch (error) {
        log(`Ha ocurrido un error con la función: ${error.name} El mensaje es: ${error.message}.`);
    }
}

function getFloatValue(str) {
    if (!str) {
        return null;
    }
    return parseFloat(str);
}