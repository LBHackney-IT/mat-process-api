.PHONY: build
build:
	docker-compose build mat-process-api

.PHONY: serve
serve:
	docker-compose up mat-process-api

.PHONY: shell
shell:
	docker-compose run mat-process-api bash

.PHONY: test
test:
	docker-compose build mat-process-api-test && docker-compose up mat-process-api-test
