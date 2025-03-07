﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Global Parameter Increase")]
    public class FMODParameterIncrease: EventHandler
    {
        [ParamRef]
        [FormerlySerializedAs("parameter")]
        public string Parameter;

        public EmitterGameEvent TriggerEvent;

        // [FormerlySerializedAs("value")]
        // public float Value;

        private FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription;
        public FMOD.Studio.PARAMETER_DESCRIPTION ParameterDesctription { get { return parameterDescription; } }

        private FMOD.RESULT Lookup()
        {
            FMOD.RESULT result = RuntimeManager.StudioSystem.getParameterDescriptionByName(Parameter, out parameterDescription);
            return result;
        }

        private void Awake()
        {
            if (string.IsNullOrEmpty(parameterDescription.name))
            {
                Lookup();
            }
        }

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (TriggerEvent == gameEvent)
            {
                TriggerParameters();
            }
        }

        public void TriggerParameters()
        {
            if (!string.IsNullOrEmpty(Parameter))
            {   
                FMOD.RESULT getResult = RuntimeManager.StudioSystem.getParameterByID(parameterDescription.id,
                      out float currentValue
                    );

                Debug.Log($"writeParameterValue: {currentValue}");
                
                currentValue += 1;

                FMOD.RESULT setResult = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, currentValue);

                Debug.Log($"writeParameterValue: {currentValue}");
                
                if (setResult != FMOD.RESULT.OK)
                {
                    RuntimeUtils.DebugLogError(string.Format(("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}"), Parameter, setResult));
                }
            }
        }
    }
}