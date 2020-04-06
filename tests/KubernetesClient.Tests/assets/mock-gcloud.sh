#!/bin/bash
SCRIPT=$(readlink -f "$0")
SCRIPTPATH=$(dirname "$SCRIPT") 
OUTPUT_JSON=$SCRIPTPATH/gcloud-config-helper.json
cat $OUTPUT_JSON
