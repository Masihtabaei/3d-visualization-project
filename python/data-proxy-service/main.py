import tomli
from typing import Union
from fastapi import FastAPI

app = FastAPI()

def parse_service_information():
    with open("pyproject.toml", mode="rb") as config:
        toml_file = tomli.load(config)
        return {
            'service name': toml_file['project']['name'],
            'service version': toml_file['project']['version'],
            'service description': toml_file['project']['description']
        }


@app.get("/")
def get_root():
    return parse_service_information()
