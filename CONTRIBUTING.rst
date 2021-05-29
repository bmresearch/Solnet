Introduction
============

First off, thank you for considering contributing to Solnet. It's people like you that will make Solnet a great tool.


Support questions
-----------------

Please don't use the issue tracker for this. The issue tracker is a tool
to address bugs and feature requests in Solnet itself. Use one of the
following resources for questions about using Solnet or issues with your
own code:

-   The `discussions`_ page
    can be used to discuss a number of different questions, including support.
-   The email blockmountainresearch(at)protonmail.com can be used for longer term
    discussion or larger issues.
		
.. _discussions: https://github.com/bmresearch/Solnet/discussions

Reporting issues
----------------

Include the following information in your post:

-   Describe what you expected to happen.
-   If possible, include a `minimal reproducible example` to help us
    identify the issue. This also helps check that the issue is not with
    your own code.
-   Describe what actually happened. Include the full traceback if there
    was an exception.
-   List your .NET and Solnet versions. If possible, check if this
    issue is already fixed in the latest releases or the latest code in
    the repository.
    
Submitting pull requests
------------------

If there is not an open issue for what you want to submit, prefer
opening one for discussion before working on a PR. 
You can work on any issue that doesn't have an open PR linked to it or
a maintainer assigned to it. No need to ask if you can work on
an issue that interests you.

Include the following in your patch:

-   Include tests if your changes add or change code. Make sure the test
    fails without your changes.
-   Update any relevant docs pages, docstrings and/or relevant code comments.
-   Add an entry in ``CHANGELOG.md``. Use the same style as other
    entries. Also include ``.. versionchanged`` inline changelogs in
    relevant docstrings.


First time setup
~~~~~~~~~~~~~~~~

-   Clone the repository.
-   Build it locally using your favorite IDE.
-   Build on top of it.


Start coding
~~~~~~~~~~~~

-   Create a branch to identify the issue you would like to work on. If
    you're submitting a bug or documentation fix, branch off of the
    latest ".x" branch.

    .. code-block:: text

        $ git fetch origin
        $ git checkout -b your-branch-name origin/2.0.x

    If you're submitting a feature addition or change, branch off of the
    "main" branch.

    .. code-block:: text

        $ git fetch origin
        $ git checkout -b your-branch-name origin/main

-   Using your favorite editor, make your changes,
    `committing as you go`_.
-   Include tests that cover any code changes you make. Make sure the
    test fails without your patch. Run the tests as described below.
-   Push your commits to your fork on GitHub and
    `create a pull request`_. Link to the issue being addressed with
    ``fixes #123`` in the pull request.

.. _committing as you go: https://dont-be-afraid-to-commit.readthedocs.io/en/latest/git/commandlinegit.html#commit-your-changes
.. _create a pull request: https://docs.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request


Running the tests
~~~~~~~~~~~~~~~~~


-   TBD


Running test coverage
~~~~~~~~~~~~~~~~~~~~~

-   TBD


Building the docs
~~~~~~~~~~~~~~~~~

-   TBD
