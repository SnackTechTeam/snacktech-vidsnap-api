resource "aws_sqs_queue" "sqs_notifica_usuario" {
  name                       = var.sqsNotificaUsuarioQueueName
  delay_seconds              = 0
  visibility_timeout_seconds = 30
  message_retention_seconds  = 345600 # 4 dias

  tags = {
    Name = var.sqsNotificaUsuarioQueueName
  }
}

output "sqs_notifica_usuario" {
  value = aws_sqs_queue.sqs_notifica_usuario.url
}

output "sqs_notifica_usuario_arn" {
  value = aws_sqs_queue.sqs_notifica_usuario.arn
}