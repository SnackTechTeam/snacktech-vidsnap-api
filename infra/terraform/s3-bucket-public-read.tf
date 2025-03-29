resource "aws_s3_bucket_policy" "public_read" {
  bucket = aws_s3_bucket.s3-bucket.id
  policy = <<POLICY
    {
    "Version": "2012-10-17",
    "Statement": [
        {
        "Effect": "Allow",
        "Principal": "*",
        "Action": "s3:GetObject",
        "Resource": "arn:aws:s3:::vidsnap-video-upload/*"
        }
    ]
    }
    POLICY
}