# Sloader Overview

Sloader consists of three libraries:

* Sloader.Config: The config lib represents the base config and the specific crawler or drop configs.
* Sloader.Result: The result lib represents the base result and the specific crawler or drop results.  
* Sloader.Engine: The Sloader.Engine, which uses the config and implements the crawler & drops. Each crawler has it's own config & result "schema". When all endpoints have been crawled the result will be "dropped" to a configured destination.
