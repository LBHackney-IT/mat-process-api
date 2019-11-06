.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose run mat-process-api dotnet build

.PHONY: serve
serve:
	docker-compose up mat-process-api

.PHONY: shell
shell:
	docker-compose run mat-process-api bash

.PHONY: test
test:
	docker-compose build mat-process-api-test && docker-compose up mat-process-api-test
