resource "aws_s3_bucket" "s3-bucket" {
  bucket = "vidsnap-video-upload" # Substitua pelo nome do bucket desejado

  tags = {
    Name        = "Meu Bucket"
    Environment = "Desenvolvimento"
  }
}