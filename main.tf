provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}

data "aws_iam_role" "ec2_container_service_role" {
  name = "ecsServiceRole"
}

data "aws_iam_role" "ecs_task_execution_role" {
  name = "ecsTaskExecutionRole"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
  parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

terraform {
  backend "s3" {
    bucket  = "hackney-mat-state-storage-s3"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/process-api/state"
  }
}

resource "aws_s3_bucket" "images_bucket" {
  bucket = "mat-process-images-non-production"
  acl    = "private"

  versioning {
    enabled = true
  }

  lifecycle = {
    prevent_destroy = true
  }
}

resource "aws_s3_bucket_public_access_block" "example" {
  bucket                  = "${aws_s3_bucket.images_bucket.id}"
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

module "development" {
  # We pin to `master` for now, until we have tagged releases of the modules.
  source                      = "github.com/LBHackney-IT/aws-mat-components-per-service-terraform.git//modules/environment/backend?ref=master"
  environment_name            = "development"
  application_name            = "process-api"                                                                                                  # Replace with your application name.
  security_group_name_prefix  = "hackney-mat-sg"                                                                                               # Replace with your security group name prefix.
  lb_prefix                   = "hackney-mat-nlb"
  host_port                   = 1002
  desired_number_of_ec2_nodes = 2
  port                        = 1002

  task_definition_environment_variables = {
    DocumentDbConnString = "${local.parameter_store}/mat-documentdb-conn-string-development"
    DatabaseName         = "${local.parameter_store}/mat-documentdb-db-name-development"
    CollectionName       = "${local.parameter_store}/mat-documentdb-collection-name-development"
  }

  task_definition_environment_variable_count = 3
  ecs_execution_role                         = "${data.aws_iam_role.ecs_task_execution_role.arn}"
  lb_iam_role_arn                            = "${data.aws_iam_role.ec2_container_service_role.arn}"
  task_definition_secrets                    = {
    AWS_SECRET_ACCESS_KEY = "${local.parameter_store}/staging-mat-api-aws-secret-key"
    AWS_ACCESS_KEY_ID     = "${local.parameter_store}/staging-mat-api-aws-access-key-id"
    bucket-name           = "${local.parameter_store}/staging-mat-images-bucket-name"
    S3_ASSUME_ROLE_ARN    = "${local.parameter_store}/staging-mat-api-assume-role-arn"
  }

  task_definition_secret_count = 0
}

module "staging" {
  # We pin to `master` for now, until we have tagged releases of the modules.
  source                                = "github.com/LBHackney-IT/aws-mat-components-per-service-terraform.git//modules/environment/backend?ref=master"
  environment_name                      = "staging"
  application_name                      = "process-api"                                                                                                  # Replace with your application name.
  security_group_name_prefix            = "hackney-mat-sg"                                                                                               # Replace with your security group name prefix.
  lb_prefix                             = "hackney-mat-nlb"
  host_port                             = 1001
  desired_number_of_ec2_nodes           = 2
  port                                  = 1001
  task_definition_environment_variables = {}

  task_definition_environment_variable_count = 0

  ecs_execution_role = "${data.aws_iam_role.ecs_task_execution_role.arn}"
  lb_iam_role_arn    = "${data.aws_iam_role.ec2_container_service_role.arn}"

  task_definition_secrets = {
    DatabaseName          = "${local.parameter_store}/mat-documentdb-db-name-staging"
    CollectionName        = "${local.parameter_store}/mat-documentdb-collection-name-staging" # Use the same db for dev and staging
    DocumentDbConnString  = "${local.parameter_store}/mat-documentdb-conn-string-staging"
    AWS_SECRET_ACCESS_KEY = "${local.parameter_store}/staging-mat-api-aws-secret-key"
    AWS_ACCESS_KEY_ID     = "${local.parameter_store}/staging-mat-api-aws-access-key-id"
    bucket-name           = "${local.parameter_store}/staging-mat-images-bucket-name"
    S3_ASSUME_ROLE_ARN    = "${local.parameter_store}/staging-mat-api-assume-role-arn"
  }

  task_definition_secret_count = 7
}

module "production" {
  # We pin to `master` for now, until we have tagged releases of the modules.
  source                                = "github.com/LBHackney-IT/aws-mat-components-per-service-terraform.git//modules/environment/backend?ref=master"
  environment_name                      = "production"
  application_name                      = "process-api"                                                                                                  # Replace with your application name.
  security_group_name_prefix            = "hackney-mat-sg"                                                                                               # Replace with your security group name prefix.
  lb_prefix                             = "hackney-mat-nlb"
  host_port                             = 1000
  desired_number_of_ec2_nodes           = 2
  port                                  = 1000
  task_definition_environment_variables = {}

  task_definition_environment_variable_count = 0

  ecs_execution_role = "${data.aws_iam_role.ecs_task_execution_role.arn}"
  lb_iam_role_arn    = "${data.aws_iam_role.ec2_container_service_role.arn}"

  task_definition_secrets = {
    DatabaseName          = "${local.parameter_store}/mat-documentdb-db-name-staging"
    CollectionName        = "${local.parameter_store}/mat-documentdb-collection-name-staging" # Use the same db for dev and staging
    DocumentDbConnString  = "${local.parameter_store}/mat-documentdb-conn-string-staging"
    AWS_SECRET_ACCESS_KEY = "${local.parameter_store}/staging-mat-api-aws-secret-key"
    AWS_ACCESS_KEY_ID     = "${local.parameter_store}/staging-mat-api-aws-access-key-id"
    bucket-name           = "${local.parameter_store}/staging-mat-images-bucket-name"
    S3_ASSUME_ROLE_ARN    = "${local.parameter_store}/staging-mat-api-assume-role-arn"
  }

  task_definition_secret_count = 7
}
