cbuffer GlobalBuffer
{
	float _UnscaledTime;
	float _UnscaledDeltaTime;
};

void UnscaledSineTime_float(out float sineTime)
{
	sineTime = sin(_UnscaledTime);
}

void UnscaledCosineTime_float(out float cosineTime)
{
	cosineTime = cos(_UnscaledTime);
}

void UnscaledDeltaTime_float(out float unscaledDeltaTime)
{
	unscaledDeltaTime = _UnscaledDeltaTime;
}

void UnscaledTime_float(out float time)
{
	time = _UnscaledTime;
}