import unittest
import os
import subprocess
import json


class Test_main_aPythonConnectedGrasshopperComponent(unittest.TestCase):
    def setUp(self):
        # Define paths
        self.current_dir = os.path.dirname(os.path.abspath(__file__))  # Current test file directory
        self.pythonProject_dir = os.path.abspath(os.path.join(self.current_dir, os.pardir, os.pardir))  # Move two directories up
        self.data_file = os.path.join(self.current_dir, 'TwinData.txt')
        self.result_file = os.path.join(self.current_dir, 'TwinResult.txt')
        self.script_path = os.path.join(self.pythonProject_dir, 'main_aPythonConnectedGrasshopperComponent.py')

    def tearDown(self):
        # Clean up result file if it exists
        if os.path.exists(self.result_file):
            os.remove(self.result_file)

    def test_script_with_shared_data(self):
        # Extract the key from TwinData.txt
        with open(self.data_file, 'r') as f:
            key = f.readline().strip()

        # Run the script with arguments
        result = subprocess.run(
            ['python', self.script_path, key, self.data_file, self.result_file],
            capture_output=True,
            text=True
        )

        # Verify that the script executed successfully
        self.assertEqual(result.returncode, 0, f"Script failed with: {result.stderr}")

        # Check the result file
        self.assertTrue(os.path.exists(self.result_file), "Result file was not created.")
        with open(self.result_file, 'r') as f:
            file_key = f.readline().strip()
            file_result = json.loads(f.read().strip())

        # Verify that the key matches
        self.assertEqual(key, file_key, "Key in the result file does not match the input key.")

        # Validate the result's structure
        self.assertIn("Matrix", file_result, "Result does not contain 'Matrix'.")
        self.assertEqual(len(file_result["Matrix"]), 2, "Matrix rows do not match expected row number.")
        self.assertEqual(len(file_result["Matrix"][0]), 5, "Matrix columns do not match expected column number.")

if __name__ == '__main__':
    unittest.main()