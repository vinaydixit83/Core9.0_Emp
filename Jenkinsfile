pipeline {
    agent any

    environment {
        DOTNET_HOME = "C:\\Program Files\\dotnet" // Adjust the path for .NET SDK on your local Jenkins server
        PATH = "D:\\Workspace\\EmpManagementV1\\Publish"
    }

    stages {
        stage('Clone Repository') {
            steps {
                echo 'Restoring dependencies...'
                bat '''
                @echo off
                "C:\\Program Files\\dotnet\\dotnet.exe" restore
                '''
            }
        }

        stage('Restore Dependencies') {
            steps {
                echo 'Restoring dependencies...'
            bat 'dotnet restore'
            }
        }

         stage('Build') {
            steps {
                echo 'Building the application...'
                bat 'C:\\Program Files\\dotnet\\dotnet.exe" build --configuration Release'
            }
        }

         stage('publish') {
            steps {
                script {
                    bat '"%DOTNET_HOME%\\dotnet.exe" publish --configuration Release --output D:\\Workspace\\EmpManagementV1\\Publish'
                }
                //echo 'Building the application...'
            }
        }

        //stage('Run Tests') {
           // steps {
             //   echo 'Running unit tests...'
             //  sh 'dotnet test'
            //}
       // }

         stage('Deploy') {
            steps {
                echo 'Publishing the application...'
                bat 'dotnet publish -c Release -o publish'

                echo 'Starting the application on localhost...'
                bat 'start cmd /c dotnet publish\\EmployeeManagement.dll'
            }
        }
    }

    post {
        success {
            echo 'Build and deployment successful!'
        }
        failure {
            echo 'Build or deployment failed.'
        }
    }
}
