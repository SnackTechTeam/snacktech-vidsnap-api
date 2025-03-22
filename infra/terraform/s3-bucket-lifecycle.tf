resource "aws_s3_bucket_lifecycle_configuration" "lifecycle" {
  bucket = aws_s3_bucket.s3-bucket.id

  rule {
    id     = "delete-old-files"
    status = "Enabled"

    expiration {
      days = 30 # Arquivos expiram após 30 dias
    }

    filter {
      prefix = "" # Aplica a todos os objetos do bucket
    }
  }
}