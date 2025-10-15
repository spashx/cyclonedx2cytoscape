#!/bin/bash
# Run CdxViz Integration Tests
# This script demonstrates how to run tests with different configurations

# Default values
EXECUTABLE_PATH=""
FORMATS="cytoscape,3dforce"
COVERAGE=false
VERBOSE=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --executable-path)
            EXECUTABLE_PATH="$2"
            shift 2
            ;;
        --formats)
            FORMATS="$2"
            shift 2
            ;;
        --coverage)
            COVERAGE=true
            shift
            ;;
        --verbose)
            VERBOSE=true
            shift
            ;;
        --help)
            echo "Usage: ./run-tests.sh [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --executable-path PATH   Path to CdxViz executable"
            echo "  --formats FORMATS        Comma-separated list of formats to test (default: cytoscape,3dforce)"
            echo "  --coverage               Enable code coverage collection"
            echo "  --verbose                Enable verbose output"
            echo "  --help                   Show this help message"
            echo ""
            echo "Examples:"
            echo "  ./run-tests.sh"
            echo "  ./run-tests.sh --coverage"
            echo "  ./run-tests.sh --formats cytoscape"
            echo "  ./run-tests.sh --executable-path /path/to/CdxViz --coverage --verbose"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

echo "=== CdxViz Integration Tests ==="

# Set executable path if provided
if [ -n "$EXECUTABLE_PATH" ]; then
    export CDXVIZ_EXECUTABLE_PATH="$EXECUTABLE_PATH"
    echo "Using executable: $EXECUTABLE_PATH"
fi

# Set formats to test
export CDXVIZ_TEST_FORMATS="$FORMATS"
echo "Testing formats: $FORMATS"

# Build test arguments
TEST_ARGS=()

if [ "$COVERAGE" = true ]; then
    echo "Code coverage enabled"
    TEST_ARGS+=("--collect:XPlat Code Coverage")
    TEST_ARGS+=("--settings")
    TEST_ARGS+=("coverlet.runsettings")
fi

if [ "$VERBOSE" = true ]; then
    TEST_ARGS+=("--logger")
    TEST_ARGS+=("console;verbosity=detailed")
fi

# Navigate to test project directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo ""
echo "Running tests..."
echo "Command: dotnet test ${TEST_ARGS[*]}"
echo ""

# Run tests
dotnet test "${TEST_ARGS[@]}"
EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "? Tests completed successfully!"
    echo ""
    echo "Test outputs are available in: $SCRIPT_DIR/bin/Debug/net9.0/TestsOutput"
    
    if [ "$COVERAGE" = true ]; then
        echo ""
        echo "Coverage reports are available in: $SCRIPT_DIR/TestResults"
    fi
else
    echo ""
    echo "? Tests failed!"
    exit $EXIT_CODE
fi
